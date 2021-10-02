using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcService1;

namespace GreetAndCalculate
{
    // A command line application.
    internal class Program
    {
        internal static async Task Main()
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            
            var serviceChoice = string.Empty;
            var possibleChoices = new HashSet<string>() {"1", "2", "3", "4", "5", "6"};
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

            switch (serviceChoice)
            {
                // greetings
                case "1":
                {
                    var greeterClient = StartGreeterClient(channel, out var name);
                    HelloReply reply = await greeterClient.SayHelloAsync(
                        new HelloRequest { Name = name });
                    Console.WriteLine("Greeting: " + reply.Message);
                    Console.WriteLine("Press any key to exit...");
                    break;
                }
                case "2":
                {
                    var greeterClient = StartGreeterClient(channel, out var name);
                    var reply = greeterClient.SayHelloManyTimes(new HelloManyTimesRequest() {Name = name});

                    while (await reply.ResponseStream.MoveNext())
                    {
                            Console.WriteLine(reply.ResponseStream.Current.Message);
                            await Task.Delay(200);
                    }
                    break;
                }
                case "3":
                {
                    var greeterClient = StartGreeterClient(channel, out var name);
                    var request = new LongHelloRequest() { Name = name };
                    var stream = greeterClient.SayLongHello();

                    foreach (var i in Enumerable.Range(1, 10))
                    {
                        await stream.RequestStream.WriteAsync(request);
                    }

                    await stream.RequestStream.CompleteAsync();
                    var response = stream.ResponseAsync;
                    Console.WriteLine(response.Result.Message);
                    break;
                }
                // calculations
                case "4":
                {
                    var calculatorClient = new Calculator.CalculatorClient(channel);

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

                    CalculationResponse calculationResponse = await calculatorClient.DoCalculationAsync(request);
                    Console.WriteLine(calculationResponse.Message);
                    break;
                }
                case "5":
                {
                    var calculatorClient = new Calculator.CalculatorClient(channel);

                    Console.Write("Enter a number: ");
                    var number = int.Parse(Console.ReadLine() ?? string.Empty);

                    var reply = calculatorClient.DecomposePrime(new PrimeDecompositionRequest() { Number = number });

                    while (await reply.ResponseStream.MoveNext())
                    {
                        Console.WriteLine(reply.ResponseStream.Current.PrimeFactor);
                        await Task.Delay(200);
                    }
                    break;
                }
                case "6":
                {
                    var calculatorClient = new Calculator.CalculatorClient(channel);
                    var stream = calculatorClient.Average();

                    Console.Write("Enter some numbers separated by commas: ");
                    var input = Console.ReadLine();
                    string[] numbers = input?.Split(",");

                    if (numbers != null)
                        foreach (var number in numbers)
                        {
                            var request = new AverageRequest() { Number = int.Parse(number) };
                            await stream.RequestStream.WriteAsync(request);
                        }

                    await stream.RequestStream.CompleteAsync();
                    var response = stream.ResponseAsync;
                    Console.WriteLine("The average of your numbers is: " + response.Result.Average);

                    break;
                }
            }

            channel.ShutdownAsync().Wait();
        }

        private static Greeter.GreeterClient StartGreeterClient(GrpcChannel channel, out string name)
        {
            var client1 = new Greeter.GreeterClient(channel);
            Console.Write("Please enter your name: ");
            name = Console.ReadLine();
            return client1;
        }
    }
}
