using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.N3;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website
{
    [TestFixture]
    public class and_obtaining_some_resource : ScenarioTest
    {
        private IHypermediaContainer Resource { get; set; }

        public override async Task TheTest()
        {
            Resource = await Client.GetResource(new Uri(Url, "resource"));
        }

        [Test]
        public void should_obtain_three_hypermedia_controls()
        {
            Resource.Should().HaveCount(3);
        }

        [Test]
        public void should_obtain_a_schema_AddAction_operations()
        {
            Resource.OfType<IHydraResource>().Where(_ => _.Operations.OfType(new Iri("http://schema.org/AddAction")).Any())
                .Should().HaveCount(2);
        }

        [Test]
        public void should_obtain_a_collection_of_events()
        {
            Resource.Collections.Where(item => Regex.IsMatch(item.Iri.ToString(), "/api/events$"))
                .Should().HaveCount(1);
        }

        protected override HydraClientFactory ConfigureClient(HydraClientFactory hydraClientFactory)
        {
            return hydraClientFactory.WithFactory(N3HypermediaProcessorFactory.Instance);
        }
    }
}
