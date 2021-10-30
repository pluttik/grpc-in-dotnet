using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GreetAndCalculate.IntegrationTests
{
    public sealed class TestServerFixture : IDisposable
    {
        private readonly WebApplicationFactory<GreetAndCalculate.Program> _factory;

        public TestServerFixture()
        {
            _factory = new WebApplicationFactory<GreetAndCalculate.Program>();
            var client = _factory.CreateDefaultClient(new ResponseVersionHandler());
            GrpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }

        public GrpcChannel GrpcChannel { get; }

        public void Dispose()
        {
            _factory.Dispose();
        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;
                return response;
            }
        }
    }
}
