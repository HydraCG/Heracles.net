using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Namespaces;
using Heracles.Rdf.GraphTransformations;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;
using RollerCaster;

namespace Given_instance_of.ApiDocumentationExtendingGraphTransformer_class
{
    [TestFixture]
    public class when_transforming
    {
        private ApiDocumentationExtendingGraphTransformer Transformer { get; set; }
        
        private IResource Resource { get; set; }
        
        private Mock<IHypermediaProcessor> Processor { get; set; }
        
        private Mock<IHypermediaProcessingOptions> Options { get; set; }
        
        private IEnumerable<ITypedEntity> Result { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Processor = new Mock<IHypermediaProcessor>(MockBehavior.Strict);
            var context = Mappings.InContextOf(hydra.Collection);
            var operation = new MulticastObject();
            context.Setup(_ => _.Create<IOperation>(It.IsAny<Iri>())).Returns(operation.ActLike<IOperation>());
            var supportedOperation = new Mock<IOperation>();
            supportedOperation.SetupGet(_ => _.Title).Returns("title");
            supportedOperation.SetupGet(_ => _.Context).Returns(context.Object);
            var supportedClass = new Mock<IClass>(MockBehavior.Strict);
            supportedClass.SetupGet(_ => _.Iri).Returns(hydra.Collection);
            supportedClass.SetupGet(_ => _.SupportedOperations)
                .Returns(new HashSet<IOperation>() { supportedOperation.Object });
            var apiDocumentation = new Mock<IApiDocumentation>(MockBehavior.Strict);
            apiDocumentation.SetupGet(_ => _.SupportedClasses).Returns(new HashSet<IClass>() { supportedClass.Object });
            Options = new Mock<IHypermediaProcessingOptions>(MockBehavior.Strict);
            Options.SetupGet(_ => _.ApiDocumentations).Returns(new[] { apiDocumentation.Object });
            Options.SetupGet(_ => _.OriginalUrl).Returns(new Uri("http://temp.uri/"));
            var proxy = new MulticastObject();
            proxy.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), context.Object);
            Resource = proxy.ActLike<IResource>();
            Resource.Type.Add(hydra.Collection);
            Transformer = new ApiDocumentationExtendingGraphTransformer();
            TheTest();
        }

        public void TheTest()
        {
            Result = Transformer.Transform(new[] { Resource }, Processor.Object, Options.Object);
        }

        [Test]
        public void should_add_API_documentation_discovered_operation()
        {
            Result.Should().Contain(Resource).Which.ActLike<IHydraResource>().Operations
                .Should().HaveCount(1).And.Subject.First().Title.Should().Be("title");
        }
    }
}