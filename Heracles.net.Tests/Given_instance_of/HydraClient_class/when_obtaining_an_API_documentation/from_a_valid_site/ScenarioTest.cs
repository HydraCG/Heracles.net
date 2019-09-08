using System.IO;
using Heracles;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation.from_a_valid_site
{
    public abstract class ScenarioTest : HydraClientTest
    {
        protected IResponse UrlResponse { get; private set; }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(BaseUrl, It.IsAny<IHttpOptions>()))
                .ReturnsAsync(UrlResponse = Return.Ok(
                    BaseUrl,
                    new MemoryStream(),
                    WithHeaders("Link", $"<{BaseUrl}api/documentation>; rel=\"{hydra.apiDocumentation}\"")));
        }
    }
}
