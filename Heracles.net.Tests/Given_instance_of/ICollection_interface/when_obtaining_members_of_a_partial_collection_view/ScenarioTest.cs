using System.Collections.Generic;
using System.Linq;
using Heracles.DataModel;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;
using RDeF.Entities;
using RollerCaster;

namespace Given_instance_of.ICollection_interface.when_obtaining_members_of_a_partial_collection_view
{
    public abstract class ScenarioTest : CollectionTest
    {
        protected IResource FirstPage { get; private set; }

        protected IResource SecondPage { get; private set; }

        protected IResource LastPage { get; private set; }

        protected IHypermediaContainer FirstBatch { get; private set; }

        protected IHypermediaContainer SecondBatch { get; private set; }
        
        protected IHypermediaContainer LastBatch { get; private set; }

        protected Mock<IResource> InitialLink { get; private set; }

        protected ICollection<IResource> InitialMembers { get; private set; }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            FirstPage = Resource.Of<IResource>(new Iri("page:1")).Object;
            SecondPage = Resource.Of<IResource>(new Iri("page:2")).Object;
            LastPage = Resource.Of<IResource>(new Iri("page:3")).Object;
            FirstBatch = CollectionOf(new Iri("view:1"), SecondPage, null, new Iri("some:item"));
            SecondBatch = CollectionOf(new Iri("view:2"), LastPage, FirstPage, new Iri("some:another-item"));
            LastBatch = CollectionOf(new Iri("view:3"), null, SecondPage, new Iri("yet:another-item"));
            InitialLink = Mock.Get(SecondPage);
            InitialMembers = new List<IResource>();
            var collection = Collection.AsDynamic();
            collection.View = ViewOf(new Iri("view:1"), SecondPage, SecondPage);
            collection.Members = InitialMembers;
            collection.Iri = FirstPage.Iri;
        }

        private IHypermediaContainer CollectionOf(Iri iri, IResource next, IResource previous, params Iri[] iris)
        {
            var result = new Mock<IHypermediaContainer>(MockBehavior.Strict);
            result.SetupGet(_ => _.Members).Returns(new HashSet<IResource>(iris.Select(_ => Resource.Of<IResource>(_).Object)));
            result.SetupGet(_ => _.View).Returns(ViewOf(iri, next, previous));
            return result.Object;
        }

        private IPartialCollectionView ViewOf(Iri iri, IResource next, IResource previous)
        {
            var view = new Mock<IPartialCollectionView>(MockBehavior.Strict);
            view.As<IEntity>().SetupGet(_ => _.Iri).Returns(iri);
            view.SetupGet(_ => _.First).Returns((ILink)null);
            view.SetupGet(_ => _.Next).Returns(next);
            view.SetupGet(_ => _.Previous).Returns(previous);
            view.SetupGet(_ => _.Last).Returns((ILink)null);
            return view.Object;
        }
    }
}
