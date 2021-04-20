using System;
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
    }
}
