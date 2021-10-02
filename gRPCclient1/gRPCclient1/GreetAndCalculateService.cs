using Grpc.Net.Client;
using GrpcService1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreetAndCalculate
{
    public static class GreetAndCalculateService
    {
        public static string AskServiceChoice()
        {
            var serviceChoice = string.Empty;
            var possibleChoices = new HashSet<string>() { "1", "2", "3", "4", "5", "6" };
            while (!possibleChoices.Contains(serviceChoice))
            {
                Console.Write("Would you like to use the regular greeting service (1), " +
                              "the server-side streaming greeting service (2), " +
                              "the client-side streaming greeting service (3), " +
                              "the calculation service (4), " +
                              "streaming prime decomposition (5) " +
                              "or calculate an average (6)? ");
                serviceChoice = Console.ReadLine();
                if (!possibleChoices.Contains(serviceChoice))
                {
                    Console.WriteLine("Please use a valid option.");
                }
            }
            return serviceChoice;
        }

        public static Greeter.GreeterClient StartGreeterClient(GrpcChannel channel, out string name)
        {
            var client1 = new Greeter.GreeterClient(channel);
            Console.Write("Please enter your name: ");
            name = Console.ReadLine();
            return client1;
        }

        public static CalculationRequest GetCalculationRequest()
        {
            Console.Write("Add (1), subtract (2) or power (3)? ");
            string answer = Console.ReadLine();
            Console.Write("First number: ");
            double firstNumber = double.Parse(Console.ReadLine() ?? string.Empty);
            Console.Write("Second number: ");
            double secondNumber = Double.Parse(Console.ReadLine() ?? string.Empty);

            CalculationRequest.Types.CalculationType calculationType = CalculationRequest.Types.CalculationType.Add;
            switch (answer)
            {
                case "1":
                    {
                        calculationType = CalculationRequest.Types.CalculationType.Add;
                        break;
                    }
                case "2":
                    {
                        calculationType = CalculationRequest.Types.CalculationType.Subtract;
                        break;
                    }
                case "3":
                    {
                        calculationType = CalculationRequest.Types.CalculationType.Power;
                        break;
                    }
            }

            var parameterList = new List<double>
                    {
                        firstNumber,
                        secondNumber
                    };

            var request = new CalculationRequest()
            {
                CalculationType = calculationType,
                Parameters = { parameterList },
            };

            return request;
        }

        public static async Task AskNumbersAndStreamAsync(AsyncClientStreamingCall<AverageRequest, AverageResponse> stream)
        {
            Console.Write("Enter some numbers separated by commas: ");
            var input = Console.ReadLine();
            string[] numbers = input?.Split(",");

            if (numbers != null)
                foreach (var number in numbers)
                {
                    var request = new AverageRequest() { Number = int.Parse(number) };
                    await stream.RequestStream.WriteAsync(request);
                }
        }
    }
}
