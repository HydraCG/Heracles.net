using System;
using System.Linq;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.SimpleVolatileResourceCache_class
{
    [TestFixture]
    public class when_operating
    {
        private static readonly Uri Uri = new Uri("some:uri");
        
        private SimpleVolatileResourceCache Cache { get; set; }
        
        private Mock<IApiDocumentation> Resource { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Cache = new SimpleVolatileResourceCache();
            Resource = new Mock<IApiDocumentation>(MockBehavior.Strict);
            TheTest();
        }

        public void TheTest()
        {
            Cache[Uri] = Resource.Object;
        }

        [Test]
        public void should_store_resource_within_cache()
        {
            Cache[Uri].Should().Be(Resource.Object);
        }

        [Test]
        public void should_provide_all_resources_of_given_type()
        {
            Cache.All<IApiDocumentation>().Should().HaveCount(1).And.Subject.First().Should().Be(Resource.Object);
        }
    }
}