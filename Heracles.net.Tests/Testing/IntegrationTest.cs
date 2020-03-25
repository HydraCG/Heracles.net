using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using HttpServer = Heracles.Server.Server;

namespace Heracles.Testing
{
    public abstract class IntegrationTest
    {
        private static HttpServer _server;
        private static CancellationTokenSource _cancellationTokenSource;

        protected Uri Url { get; } = new Uri($"http://localhost:{HttpServer.ServerPort}/");

        protected IHydraClient Client { get; private set; }

        public virtual Task TheTest()
        {
            return Task.CompletedTask;
        }

        [OneTimeSetUp]
        public void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _server = new HttpServer();
            _server.Start(_cancellationTokenSource.Token);
        }

        [SetUp]
        public async Task Setup()
        {
            var hydraClientFactory = HydraClientFactory
                .Configure()
                .WithDefaults()
                .WithApiDocumentationsFetchedAndExtended();
            Client = ConfigureClient(hydraClientFactory).AndCreate();
            ScenarioSetup();
            await TheTest();
        }

        [OneTimeTearDown]
        public void Destroy()
        {
            _cancellationTokenSource.Cancel();
        }

        protected virtual void ScenarioSetup()
        {
        }

        protected virtual HydraClientFactory ConfigureClient(HydraClientFactory hydraClientFactory)
        {
            return hydraClientFactory;
        }
    }
}
