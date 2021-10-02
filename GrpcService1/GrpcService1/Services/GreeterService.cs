using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcService1;
using Microsoft.Extensions.Logging;

namespace GrpcServiceGreetAndCalculate.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        // implementation of example of a remote procedure call without streaming
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Said 'Hello' to " + request.Name);
            return Task.FromResult(new HelloReply
            {
                Message = "Hello, " + request.Name
            });
        }

        // implementation of example of a remote procedure call that is server-side streaming 
        public override async Task SayHelloManyTimes(HelloManyTimesRequest request, IServerStreamWriter<HelloManyTimesReply> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request: ");
            Console.WriteLine(request.ToString());

            foreach (int i in Enumerable.Range(1, 10))
            {
                string result = string.Format("Hello, {0}, {1}", request.Name, i.ToString());
                await responseStream.WriteAsync(new HelloManyTimesReply() {Message = result});
            }
        }

        // implementation of example of a remote procedure call that is client-side streaming 
        public override async Task<LongHelloReply> SayLongHello(IAsyncStreamReader<LongHelloRequest> requestStream, ServerCallContext context)
        {
            var result = "";
            while (await requestStream.MoveNext())
            {
                result += $"Hello {requestStream.Current.Name}, {Environment.NewLine}";
            }
            return new LongHelloReply() { Message = result };
        }
    }
}
