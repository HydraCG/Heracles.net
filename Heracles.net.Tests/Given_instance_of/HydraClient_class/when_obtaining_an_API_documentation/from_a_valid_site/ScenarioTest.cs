﻿using System.IO;
using System.Threading;
using Heracles;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;

namespace Given_instance_of.HydraClient_class.when_obtaining_an_API_documentation.from_a_valid_site
{
    public abstract class ScenarioTest : when_obtaining_an_API_documentation.ScenarioTest
    {
        protected IResponse UrlResponse { get; private set; }

        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(BaseUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(UrlResponse = Return.Ok(
                    BaseUrl,
                    new MemoryStream(),
                    WithHeaders("Link", $"<{BaseUrl}api/documentation>; rel=\"{hydra.apiDocumentation}\"")));
        }
    }
}
