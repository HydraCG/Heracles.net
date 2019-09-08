using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;
using RollerCaster;

namespace Given_instance_of.JsonLdHypermediaProcessor_class.when_parsing.JSON_LD_response
{
    [TestFixture]
    public class with_nested_resources : ScenarioTest
    {
        private Stream _inputJsonLd;

        protected override Stream InputJsonLd
        {
            get { return _inputJsonLd; }
        }

        private IHydraResource Markus { get; set; }

        private IHydraResource Karol { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            Markus = Result.OfType<IHydraResource>().Where(_ => _.Iri.ToString().Contains("/markus")).First();
            Karol = Markus.Links.WithRelationOf(schema.knows).First().Target.ActLike<IHydraResource>();
        }

        [Test]
        public void should_gain_access_to_outer_resources_link()
        {
            Markus.Links.WithRelationOf(foaf.homePage).First().Target.Iri.Should().Be(
                new Iri((string)Api.People.Markus + "/home-page"));
        }
        
        [Test]
        public void should_gain_access_to_inner_resources_link()
        {
            Karol.Links.WithRelationOf(foaf.homePage).First().Target.Iri.Should().Be(new Iri((string)Api.People.Karol + "/home-page"));
        }
        
        [Test]
        public void should_have_a_nested_resources_link()
        {
            Markus.Links.WithRelationOf(schema.knows).First().Target.Should().Be(Karol);
        }

        protected override void ScenarioSetup()
        {
            _inputJsonLd = GetResourceNamed("nestedResourcesInput.json");
            Response.SetupGet(_ => _.Url).Returns(Api.People.Markus);
            base.ScenarioSetup();
        }
    }
}
