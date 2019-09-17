using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.Testing;

namespace Having_a_Hydra_client.while_browsing_a_website
{
    public abstract class ScenarioTest : IntegrationTest
    {
        protected IApiDocumentation ApiDocumentation { get; private set; }

        public override async Task TheTest()
        {
            ApiDocumentation = await Client.GetApiDocumentation(Url);
        }
    }
}
