using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.PartialCollectionCrawler_class
{
    public abstract class PartialCollectionCrawlerTest
    {
        private int _part;

        protected PartialCollectionCrawler Crawler { get; set; }

        protected Mock<IPartialCollectionIterator> Iterator { get; set; }

        protected Mock<ICollection> InitialCollection { get; set; }

        protected IEnumerable<IResource> Result { get; set; }

        public abstract Task TheTest();

        [SetUp]
        public async Task Setup()
        {
            _part = 2;
            Iterator = new Mock<IPartialCollectionIterator>(MockBehavior.Strict);
            Iterator.SetupGet(_ => _.FirstPartIri).Returns(() => new Iri($"page:{_part = 1}"));
            Iterator.SetupGet(_ => _.LastPartIri).Returns(() => new Iri($"page:{_part = 4}"));
            Iterator.SetupGet(_ => _.CurrentPartIri).Returns(() => MakePartIri(0));
            Iterator.SetupGet(_ => _.NextPartIri).Returns(() => MakePartIri(1));
            Iterator.SetupGet(_ => _.PreviousPartIri).Returns(() => MakePartIri(-1));
            Iterator.Setup(_ => _.GetFirstPart()).ReturnsAsync(new[] { Resource.Of<IResource>(new Iri("item:1")).Object });
            Iterator.Setup(_ => _.GetLastPart()).ReturnsAsync(new[] { Resource.Of<IResource>(new Iri("item:4")).Object });
            InitialCollection = new Mock<ICollection>(MockBehavior.Strict);
            InitialCollection.SetupGet(_ => _.Iri).Returns(new Iri("collection"));
            InitialCollection.SetupGet(_ => _.Members).Returns(new HashSet<IResource>() { Resource.Of<IResource>(new Iri("item:2")).Object });
            InitialCollection.SetupGet(_ => _.View).Returns(() => Resource.Of<IPartialCollectionView>(new Iri($"page:{_part}")).Object);
            InitialCollection.Setup(_ => _.GetIterator()).Returns(Iterator.Object);
            Crawler = PartialCollectionCrawler.From(InitialCollection.Object);
            ScenarioSetup();
            await TheTest();
        }

        protected virtual void ScenarioSetup()
        {
        }

        protected Iri MakePartIri(int direction)
        {
            return (direction > 0 && _part >= 4) || (direction < 0 && _part <= 1)
                ? null
                : new Iri($"page:{_part += direction}");
        }

        protected ICrawlingOptions With(
            bool? rewind = null,
            CrawlingDirection? direction = null,
            int? memberLimit = null,
            int? requestLimit = null)
        {
            var result = new Mock<ICrawlingOptions>(MockBehavior.Strict);
            result.SetupGet(_ => _.Rewind).Returns(rewind);
            result.SetupGet(_ => _.Direction).Returns(direction);
            result.SetupGet(_ => _.MemberLimit).Returns(memberLimit);
            result.SetupGet(_ => _.RequestLimit).Returns(requestLimit);
            return result.Object;
        }
    }
}
