using Grpc.Net.Client;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace GreetAndCalculate.UnitTests
{
    public class GreetAndCalculateServiceTests
    {
        IGreetAndCalculateService _greetAndCalculateService;
        Mock<IGetUserInput> _getUserInput;

        [SetUp]
        public void Setup()
        {
            _getUserInput = new Mock<IGetUserInput>();
            _getUserInput.Setup(g => g.GetInput()).Returns("1");
            _greetAndCalculateService = new GreetAndCalculateService(_getUserInput.Object);
        }

        [Test]
        public void AskServiceChoice_ShouldReturn_CorrectAnswer()
        {
            // act
            var answer = _greetAndCalculateService.AskServiceChoice();

            // assert
            Assert.AreEqual("1", answer);
        }

        [Test]
        public void GetCalculationRequest_ShouldReturn_ExpectedRequest()
        {
            //arrange
            var expectedRequest = new GrpcService1.CalculationRequest()
            {
                CalculationType = GrpcService1.CalculationRequest.Types.CalculationType.Add,
            };
            expectedRequest.Parameters.Add(1);
            expectedRequest.Parameters.Add(1);

            // act
            var actualRequest = _greetAndCalculateService.GetCalculationRequest();

            // assert
            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void AskNumbers_ShouldReturn_CorrectNumbers()
        {
            // act
            var answer = _greetAndCalculateService.AskNumbers();

            // assert
            Assert.AreEqual(new string[] { "1" }, answer);
        }
    }
}