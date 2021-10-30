using NUnit.Framework;

namespace GreetAndCalculate.IntegrationTests
{
    public class IntegrationTests
    {
        private readonly IGreetAndCalculateService _greetAndCalculateService;

        public IntegrationTests(TestServerFixture testServerFixture)
        {
            var channel = testServerFixture.GrpcChannel;
            _greetAndCalculateService = channel.CreateGrpcService<IGreetAndCalculateService>();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}