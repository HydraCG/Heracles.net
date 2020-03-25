using System;
using System.Collections.Generic;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Namespaces;
using Heracles.Rdf.GraphTransformations;
using Heracles.Testing;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using RDeF.Entities;
using RDeF.Mapping;
using RollerCaster;
using Tavis.UriTemplates;

namespace Given_instance_of.EntryPointCorrectingGraphTransformer_class
{
    [TestFixture]
    public class when_transforming
    {
        private EntryPointCorrectingGraphTransformer Transformer { get; set; }
        
        private Mock<IHypermediaProcessingOptions> Options { get; set; }
        
        private Mock<IResponse> EntryPoint { get; set; }
        
        private Mock<IHypermediaProcessor> Processor { get; set; }
        
        private IApiDocumentation ApiDocumentation { get; set; }
        
        private IEnumerable<ITypedEntity> Result { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Mappings.For<IApiDocumentation>();
            Transformer = new EntryPointCorrectingGraphTransformer();
            Options = new Mock<IHypermediaProcessingOptions>(MockBehavior.Strict);
            EntryPoint = new Mock<IResponse>(MockBehavior.Strict);
            Options.SetupGet(_ => _.AuxiliaryOriginalUrl).Returns(new Uri("http://temp.uri/"));
            Options.SetupGet(_ => _.AuxiliaryResponse).Returns(EntryPoint.Object);
            Processor = new Mock<IHypermediaProcessor>(MockBehavior.Strict);
            Processor.Setup(_ => _.Supports(It.IsAny<IResponse>())).Returns(Level.FullSupport);
            var context = Mappings.InContextOf(hydra.ApiDocumentation);
            var entryPoint = new MulticastObject();
            entryPoint.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Iri)), new Iri("http://temp.uri/"));
            context.Setup(_ => _.Create<IResource>(It.IsAny<Iri>())).Returns(entryPoint.ActLike<IResource>());
            var proxy = new MulticastObject();
            proxy.SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), context.Object);
            ApiDocumentation = proxy.ActLike<IApiDocumentation>();
            ApiDocumentation.Type.Add(hydra.ApiDocumentation);
            TheTest();
        }

        public void TheTest()
        {
            Result = Transformer.Transform(new[] { ApiDocumentation }, Processor.Object, Options.Object);
        }

        [Test]
        public void should_check_processor_supports_the_initial_request()
        {
            Processor.Verify(_ => _.Supports(EntryPoint.Object), Times.Once);
        }

        [Test]
        public void should_setup_an_entrypoint()
        {
            ApiDocumentation.EntryPoint.Should().BeAssignableTo<IResource>()
                .Which.Iri.Should().Be(new Iri("http://temp.uri/"));
        }
    }
}