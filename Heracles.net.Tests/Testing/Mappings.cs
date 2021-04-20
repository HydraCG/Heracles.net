using System;
using System.Threading;
using System.Threading.Tasks;
using Heracles.JsonLd;
using Heracles.Namespaces;
using Heracles.Rdf;
using Heracles.Rdf.GraphTransformations;
using Moq;
using RDeF.Entities;
using RDeF.Mapping;

namespace Heracles.Testing
{
    public static class Mappings
    {
        public static void For<T>() where T : class
        {
            new JsonLdHypermediaProcessor(
                new Mock<IOntologyProvider>().Object,
                HttpCall,
                new Mock<IGraphTransformer>().Object);
        }

        public static Mock<IEntityContext> InContextOf(Iri iri)
        {
            var @class = new Mock<IStatementMapping>(MockBehavior.Strict);
            @class.SetupGet(_ => _.Term).Returns(iri);
            var mapping = new Mock<IEntityMapping>(MockBehavior.Strict);
            mapping.SetupGet(_ => _.Classes).Returns(new[] { @class.Object });
            var mappings = new Mock<IMappingsRepository>(MockBehavior.Strict);
            mappings.Setup(_ => _.FindEntityMappingFor(It.IsAny<IEntity>(), It.IsAny<Type>()))
                .Returns(mapping.Object);
            var context = new Mock<IEntityContext>(MockBehavior.Strict);
            context.SetupGet(_ => _.Mappings).Returns(mappings.Object);
            return context;
        }

        private static Task<IResponse> HttpCall(Uri url, IHttpOptions options, CancellationToken cancellationToken)
        {
            return Task.FromException<IResponse>(new NotImplementedException());
        }
    }
}