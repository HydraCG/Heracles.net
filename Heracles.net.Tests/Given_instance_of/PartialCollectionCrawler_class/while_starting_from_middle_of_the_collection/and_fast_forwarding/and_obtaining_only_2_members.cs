using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.PartialCollectionCrawler_class.while_starting_from_middle_of_the_collection.and_fast_forwarding
{
    [TestFixture]
    public class and_obtaining_only_2_members : ScenarioTest
    {
        public override async Task TheTest()
        {
            Result = await Crawler.GetMembers(With(memberLimit: 2));
        }

        [Test]
        public void should_obtain_2_members()
        {
            Result.Select(_ => _.Iri)
                .Should().BeEquivalentTo(new Iri("item:2"), new Iri("item:3"));
        }

        [Test]
        public void should_request_page_3()
        {
            Iterator.Verify(_ => _.GetNextPart(), Times.Once);
        }

        [Test]
        public void should_not_rewind_back_to_page_1()
        {
            Iterator.Verify(_ => _.GetFirstPart(), Times.Never);
        }
    }
}
