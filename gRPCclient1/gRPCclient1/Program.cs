using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcService1;

namespace GrpcGreeterClient
{
    // A command line application.
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            
            var serviceChoice = string.Empty;
            while (serviceChoice != "1" && serviceChoice != "2")
            {
                Console.Write("Would you like to use the greeting service (1) or the calculation service (2)? ");
                serviceChoice = Console.ReadLine();
                if (serviceChoice != "1" && serviceChoice != "2")
                {
                    Console.WriteLine("Please use a valid option.");
                }
            }

            switch (serviceChoice)
            {
                case "1":
                {
                    var client1 = new Greeter.GreeterClient(channel);

                    Console.Write("Please enter your name: ");
                    string name = Console.ReadLine();
                    HelloReply reply = await client1.SayHelloAsync(
                        new HelloRequest { Name = name });
                    Console.WriteLine("Greeting: " + reply.Message);
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    break;
                }
                case "2":
                {
                    var client2 = new Calculator.CalculatorClient(channel);

                    Console.Write("Add (1), subtract (2) or power (3)? ");
                    string answer = Console.ReadLine();
                    Console.Write("First number: ");
                    double firstNumber = Double.Parse(Console.ReadLine());
                    Console.Write("Second number: ");
                    double secondNumber = Double.Parse(Console.ReadLine());

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

                    CalculationResponse calculationResponse = await client2.DoCalculationAsync(request);
                    Console.WriteLine(calculationResponse.Message);
                    break;
                }
            }
        }
    }
}
