using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Rdf;
using RDeF.Serialization;

namespace Heracles.N3
{
    /// <summary>Provides an N3 based implementation of the <see cref="IHypermediaProcessor" /> interface.</summary>
    public class N3HypermediaProcessor : HypermediaProcessorBase
    {
        private const string N3 = "text/n3";
        private const string Turtle = "text/turtle";
        private const string NTriples = "application/n-triples";

        private static readonly string[] MediaTypes = { N3, Turtle, NTriples };
        
        /// <summary>Initializes a new instance of the <see cref="N3HypermediaProcessor" /> class.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        /// <param name="httpCall">HTTP call facility.</param>
        public N3HypermediaProcessor(IOntologyProvider ontologyProvider, HttpCallFacility httpCall)
            : base(ontologyProvider, httpCall)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<string> SupportedMediaTypes
        {
            get { return MediaTypes; }
        }
        
        /// <inheritdoc />
        public override Level Supports(IResponse response)
        {
            var result = Level.None;
            if (response != null)
            {
                var matchingMediaTypes =
                    from contentType in response.Headers["Content-Type"]
                    join supportedMediaType in SupportedMediaTypes on contentType equals supportedMediaType
                    select supportedMediaType;
                if (matchingMediaTypes.Any())
                {
                    result = Level.FullSupport;
                }
            }

            return result;
        }

        /// <inheritdoc />
        protected override Task<IRdfReader> CreateRdfReader(IResponse response, CancellationToken cancellationToken)
        {
            return Task.FromResult<IRdfReader>(new N3Reader());
        }

        /// <inheritdoc />
        protected override IRdfWriter CreateRdfWriter()
        {
            return new N3Writer();
        }
    }
}
