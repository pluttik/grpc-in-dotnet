using Grpc.Core;
using GrpcService1;
using System.Threading.Tasks;

namespace GreetAndCalculate
{
    public interface IGreetAndCalculateService
    {
        string AskServiceChoice();

        public CalculationRequest GetCalculationRequest();

        string[] AskNumbers();
    }
}
