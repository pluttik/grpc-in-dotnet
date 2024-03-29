﻿using Grpc.Net.Client;
using GrpcService1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreetAndCalculate
{
    public class GreetAndCalculateService : IGreetAndCalculateService
    {
        private IGetUserInput _getUserInput;

        public GreetAndCalculateService(IGetUserInput getUserInput)
        {
            _getUserInput = getUserInput;
        }

        public string AskServiceChoice()
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
                serviceChoice = _getUserInput.GetInput();
                if (!possibleChoices.Contains(serviceChoice))
                {
                    Console.WriteLine("Please use a valid option.");
                }
            }
            return serviceChoice;
        }

        public CalculationRequest GetCalculationRequest()
        {
            Console.Write("Add (1), subtract (2) or power (3)? ");
            string answer = _getUserInput.GetInput();
            Console.Write("First number: ");
            double firstNumber = double.Parse(_getUserInput.GetInput() ?? string.Empty);
            Console.Write("Second number: ");
            double secondNumber = double.Parse(_getUserInput.GetInput() ?? string.Empty);

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

        public string[] AskNumbers()
        {
            Console.Write("Enter some numbers separated by commas: ");
            var input = _getUserInput.GetInput();
            string[] numbers = input?.Split(",");

            if (numbers == null)
            {
                numbers = Array.Empty<string>();
            }

            return numbers;
        }
    }
}
