using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles;
using Heracles.DataModel;
using Heracles.Rdf;
using Moq;
using NUnit.Framework;
using RollerCaster;

namespace Given_instance_of.ICollection_interface
{
    public abstract class CollectionTest
    {
        protected Mock<IHydraClient> Client { get; private set; }

        protected ICollection Collection { get; private set; }

        protected ICollection<IResource> Result { get; private set; }

        public virtual Task TheTest()
        {
            return Task.CompletedTask;
        }

        [SetUp]
        public async Task Setup()
        {
            Result = new List<IResource>();
            Client = new Mock<IHydraClient>(MockBehavior.Strict);
            var proxy = new MulticastObject();
            proxy.SetProperty(HypermediaProcessorBase.ClientPropertyInfo, Client.Object);
            Collection = proxy.ActLike<ICollection>();
            ScenarioSetup();
            await TheTest();
        }

        protected virtual void ScenarioSetup()
        {
        }
    }
}
