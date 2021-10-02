using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService1.Services
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        private readonly ILogger<CalculatorService> _logger;

        public CalculatorService(ILogger<CalculatorService> logger)
        {
            _logger = logger;
        }

        // implementation of example of a remote procedure call without streaming, does a calculation
        public override Task<CalculationResponse> DoCalculation(CalculationRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Did calculation of type: " + request.CalculationType);
            double answer;
            switch (request.CalculationType)
            {
                case CalculationRequest.Types.CalculationType.Add:
                {
                    answer = request.Parameters[0] + request.Parameters[1];
                    break;
                }
                case CalculationRequest.Types.CalculationType.Subtract:
                {
                    answer = request.Parameters[0] - request.Parameters[1];
                    break;
                }
                case CalculationRequest.Types.CalculationType.Power:
                {
                    answer = Math.Pow(request.Parameters[0], request.Parameters[1]);
                    break;
                }
                default:
                {
                    answer = 0;
                    break;
                }
            }
            return Task.FromResult(new CalculationResponse()
            {
                Message = "The answer is: " + answer,
            });
        }

        // implementation of example of a remote procedure call that is server-side streaming, does a streaming prime decomposition
        public override async Task DecomposePrime(PrimeDecompositionRequest request, IServerStreamWriter<PrimeDecompositionResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request: ");
            Console.WriteLine(request.ToString());

            var k = 2;
            var number = request.Number;

            while (number > 1)
            {
                if (number % k == 0)
                {
                    await responseStream.WriteAsync(new PrimeDecompositionResponse() { PrimeFactor = k });
                    number /= k;
                }
                else
                {
                    k += 1;
                }
            }
        }

        // implementation of example of a remote procedure call that is client-side, calculates average
        public override async Task<AverageResponse> Average(IAsyncStreamReader<AverageRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request: ");

            var total = 0;
            var number = 0;
            while (await requestStream.MoveNext())
            {
                total += requestStream.Current.Number;
                number++;
            }

            return new AverageResponse() { Average = (double)total / number };
        }
    }
}
