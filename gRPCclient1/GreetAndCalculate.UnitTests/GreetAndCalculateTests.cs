using Moq;
using NUnit.Framework;

namespace GreetAndCalculate.UnitTests
{
    public class GreetAndCalculateTests
    {
        IGreetAndCalculateService greetAndCalculateService;
        Mock<IGetUserInput> getUserInput;

        [SetUp]
        public void Setup()
        {
            getUserInput = new Mock<IGetUserInput>();
            getUserInput.Setup(g => g.GetInput()).Returns("1");
            greetAndCalculateService = new GreetAndCalculateService(getUserInput.Object);
        }

        [TearDown]
        public void CleanupTest()
        {  
        }

        [Test]
        public void AskServiceChoice_ShouldReturn_CorrectAnswerAsync()
        {
            // arrange
            

            // act
            var answer = greetAndCalculateService.AskServiceChoice();

            // assert
            Assert.AreEqual("1", answer);
        }
    }
}