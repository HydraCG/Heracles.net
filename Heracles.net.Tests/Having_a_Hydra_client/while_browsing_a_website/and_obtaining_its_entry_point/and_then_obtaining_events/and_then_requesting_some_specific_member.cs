using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Testing;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_events
{
    [TestFixture]
    public class and_then_requesting_some_specific_member : ScenarioTest
    {
        private IHypermediaContainer Member { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            Member = await Client.GetResource(Events.Members.First());
        }

        [Test]
        public void should_have_DELETE_operation_available()
        {
            Member.Operations.OfType(new Iri("http://schema.org/DeleteAction")).FirstOrDefault().Should().NotBeNull();
        }

        [Test]
        public void should_have_DELETE_operation_applicable()
        {
            Member.Operations.OfType(new Iri("http://schema.org/DeleteAction")).First().Target.Iri
                .Should().Be(Events.Members.First().Iri);
        }
    }
}
