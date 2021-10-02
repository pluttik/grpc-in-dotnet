using Grpc.Core;
using Grpc.Net.Client;
using GrpcService1;
using System.Threading.Tasks;

namespace GreetAndCalculate
{
    public interface IGreetAndCalculateService
    {
        string AskServiceChoice();

        Greeter.GreeterClient StartGreeterClient(GrpcChannel channel, out string name);

        public CalculationRequest GetCalculationRequest();

        Task AskNumbersAndStreamAsync(AsyncClientStreamingCall<AverageRequest, AverageResponse> stream);
    }
}
