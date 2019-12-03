using System;

namespace Given_instance_of.HydraClient_class.when_fetching_a_resource
{
    public abstract class ScenarioTest : HydraClientTest
    {
        protected Uri ResourceUrl { get; } = new Uri("http://temp.uri/resource2");
    }
}
