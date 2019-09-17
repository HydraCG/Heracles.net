using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.ICollection_interface.when_obtaining_members_of_a_partial_collection_view
{
    [TestFixture]
    public class by_following_next_links : ScenarioTest
    {
        public override async Task TheTest()
        {
            var iterator = Collection.GetIterator();
            while (iterator.HasNextPart)
            {
                foreach (var member in await iterator.GetNextPart())
                {
                    Result.Add(member);
                }
            }
        }

        [Test]
        public void should_not_call_the_client_for_first_part()
        {
            Client.Verify(_ => _.GetResource(FirstPage.Iri), Times.Never);
        }

        [Test]
        public void should_call_the_client_for_second_part()
        {
            Client.Verify(_ => _.GetResource(SecondPage.Iri, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void should_call_the_client_for_last_part()
        {
            Client.Verify(_ => _.GetResource(LastPage.Iri, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void should_provide_correct_result()
        {
            Result.Should().BeEquivalentTo(
                SecondBatch.Members.First(),
                LastBatch.Members.First());
        }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            InitialMembers.Add(FirstBatch.Members.First());
            var calls = 0;
            Client.Setup(_ => _.GetResource(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns<Uri, CancellationToken>((uri, token) => Task.FromResult(++calls == 1 ? SecondBatch : LastBatch));
        }
    }
}
