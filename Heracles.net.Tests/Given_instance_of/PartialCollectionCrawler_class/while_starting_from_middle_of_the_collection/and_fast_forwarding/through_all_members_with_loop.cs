using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.PartialCollectionCrawler_class.while_starting_from_middle_of_the_collection.and_fast_forwarding
{
    [TestFixture]
    public class through_all_members_with_loop : ScenarioTest
    {
        public override async Task TheTest()
        {
            Result = await Crawler.GetMembers(With(rewind: true));
        }

        [Test]
        public void should_obtain_all_members_in_the_correct_order()
        {
            Result.Select(_ => _.Iri)
                .Should().BeEquivalentTo(new Iri("item:2"), new Iri("item:3"), new Iri("item:4"), new Iri("item:1"));
        }

        [Test]
        public void should_request_an_iterator()
        {
            InitialCollection.Verify(_ => _.GetIterator(), Times.Once);
        }

        [Test]
        public void should_request_page_3_and_4()
        {
            Iterator.Verify(_ => _.GetNextPart(), Times.Exactly(2));
        }

        [Test]
        public void should_rewind_back_to_page_1()
        {
            Iterator.Verify(_ => _.GetFirstPart(), Times.Once);
        }
    }
}
