using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Heracles;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.JsonLdHypermediaProcessor_class.when_parsing
{
    [TestFixture]
    public class JSON_response : JsonLdHypermediaProcessorTest
    {
        private Stream JsonLdContext { get; set; }

        [Test]
        public void should_obtain_JSON_LD_context()
        {
            HttpCall.Verify(
                _ => _.HttpCall(
                    new Uri(new Uri("http://temp.uri/api", UriKind.Absolute), "context.jsonld"),
                    It.Is<IHttpOptions>(opts => opts.Headers["Accept"] == "application/ld+json"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            JsonLdContext = GetResourceNamed("context.json");
            HttpCall
                .Setup(_ => _.HttpCall(It.IsAny<Uri>(), It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .Returns<Uri, IHttpOptions, CancellationToken>((uri, options, token) => Task.FromResult(Return.Ok(uri, JsonLdContext)));
            ResponseHeaders.SetupGet(_ => _["Content-Type"])
                .Returns(new[] { "application/json" });
            ResponseHeaders.SetupGet(_ => _["Link"])
                .Returns(new[] { "<context.jsonld>; rel=\"http://www.w3.org/ns/json-ld#context\"; type=\"application/ld+json\"" });
            var responseBody = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            responseBody.Seek(0, SeekOrigin.Begin);
            Response.Setup(_ => _.Url).Returns(new Uri("http://temp.uri/api", UriKind.Absolute));
            Response.Setup(_ => _.GetBody(It.IsAny<CancellationToken>())).ReturnsAsync(responseBody);
        }
    }
}
