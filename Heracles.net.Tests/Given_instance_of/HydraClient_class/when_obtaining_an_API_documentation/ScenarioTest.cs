using System;
using Heracles.DataModel;
using Moq;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation
{
    public class ScenarioTest : HydraClientTest
    {
        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            Cache.SetupGet(_ => _[It.IsAny<Uri>()]).Returns((IResource)null);
        }
    }
}