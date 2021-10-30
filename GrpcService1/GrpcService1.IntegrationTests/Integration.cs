using Grpc.Net.Client;
using NUnit.Framework;
using System.Net.Http;

namespace GrpcService1.IntegrationTests
{
    public class Integration
    {
        private static HttpClient _client;
        private static CustomWebApplicationFactory _factory;
        private GrpcChannel _grpcChannel;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateDefaultClient();
            _grpcChannel = GrpcChannel.ForAddress(_client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = _client,
            });
        }

        [Test]
        public void GreeterService_SayHelloShould_ReturnCorrectMessage()
        {
            // arrange
            var greeterTestClient = new Greeter.GreeterClient(_grpcChannel);

            // act
            var response = greeterTestClient.SayHello(new HelloRequest() { Name = "TestName" });

            // assert
            Assert.AreEqual("Hello, TestName", response.Message);
        }
    }
}