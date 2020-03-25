using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Rdf.GraphTransformations;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of.CompoundGraphTransformer_class
{
    [TestFixture]
    public class when_transforming
    {
        private Mock<IResource> Resource { get; set; }
        
        private Mock<IHypermediaProcessor> Processor { get; set; }
        
        private Mock<IGraphTransformer> PrimaryGraphTransformer { get; set; }
        
        private Mock<IGraphTransformer> SecondaryGraphTransformer { get; set; }
        
        private CompoundGraphTransformer Transformer { get; set; }
        
        private IEnumerable<ITypedEntity> Result { get; set; }

        [SetUp]
        public void Setup()
        {
            Resource = new Mock<IResource>(MockBehavior.Strict);
            Processor = new Mock<IHypermediaProcessor>(MockBehavior.Strict);
            PrimaryGraphTransformer = new Mock<IGraphTransformer>(MockBehavior.Strict);
            PrimaryGraphTransformer.Setup(
                    _ => _.Transform(
                        It.IsAny<IEnumerable<ITypedEntity>>(),
                        It.IsAny<IHypermediaProcessor>(),
                        It.IsAny<HypermediaProcessingOptions>()))
                .Returns<IEnumerable<ITypedEntity>, IHypermediaProcessor, HypermediaProcessingOptions>(
                    (resources, processor, options) => resources);
            SecondaryGraphTransformer = new Mock<IGraphTransformer>(MockBehavior.Strict);
            SecondaryGraphTransformer.Setup(
                    _ => _.Transform(
                        It.IsAny<IEnumerable<ITypedEntity>>(),
                        It.IsAny<IHypermediaProcessor>(),
                        It.IsAny<HypermediaProcessingOptions>()))
                .Returns<IEnumerable<ITypedEntity>, IHypermediaProcessor, HypermediaProcessingOptions>(
                    (resources, processor, options) => resources);
            Transformer = new CompoundGraphTransformer(
                new[] { PrimaryGraphTransformer.Object, SecondaryGraphTransformer.Object });
            TheTest();
        }

        [Test]
        public void TheTest()
        {
            Result = Transformer.Transform(new[] { Resource.Object }, Processor.Object);
        }

        [Test]
        public void should_call_all_underlying_graph_transformers()
        {
            PrimaryGraphTransformer.Verify(
                _ => _.Transform(
                    It.Is<IEnumerable<ITypedEntity>>(resources => resources.First() == Resource.Object),
                    Processor.Object,
                    null),
                Times.Once);
           
            SecondaryGraphTransformer.Verify(
                _ => _.Transform(
                    It.Is<IEnumerable<ITypedEntity>>(resources => resources.First() == Resource.Object),
                    Processor.Object,
                    null),
                Times.Once);
        }

        [Test]
        public void should_provide_resulting_resources()
        {
            Result.Should().BeEquivalentTo(Resource.Object);
        }
    }
}