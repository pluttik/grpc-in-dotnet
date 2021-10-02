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
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IGetUserInput getUserInput = new GetUserInput();
            IGreetAndCalculateService greetAndCalculateService = new GreetAndCalculateService(getUserInput);

            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            
            var serviceChoice = greetAndCalculateService.AskServiceChoice();

            switch (serviceChoice)
            {
                // greetings
                case "1":
                {
                    var greeterClient = greetAndCalculateService.StartGreeterClient(channel, out var name);
                    HelloReply reply = await greeterClient.SayHelloAsync(
                        new HelloRequest { Name = name });
                    Console.WriteLine("Greeting: " + reply.Message);
                    Console.WriteLine("Press any key to exit...");
                    break;
                }
                case "2":
                {
                    var greeterClient = greetAndCalculateService.StartGreeterClient(channel, out var name);
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
                    var greeterClient = greetAndCalculateService.StartGreeterClient(channel, out var name);
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

                    var request = greetAndCalculateService.GetCalculationRequest();

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

                    await greetAndCalculateService.AskNumbersAndStreamAsync(stream);

                    await stream.RequestStream.CompleteAsync();
                    var response = stream.ResponseAsync;
                    Console.WriteLine("The average of your numbers is: " + response.Result.Average);

                    break;
                }
            }

            channel.ShutdownAsync().Wait();
        }

       
    }
}
