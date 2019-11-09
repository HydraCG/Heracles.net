using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.Rdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RDeF.Serialization;

namespace Heracles.JsonLd
{
    /// <summary>Provides a JSON-LD based implementation of the <see cref="IHypermediaProcessor" /> interface.</summary>
    public class JsonLdHypermediaProcessor : HypermediaProcessorBase
    {
        private const string JsonLdContext = "http://www.w3.org/ns/json-ld#context";
        
        private const string JsonLd = "application/ld+json";
        private const string Json = "application/json";
        
        private static readonly string[] MediaTypes = { JsonLd, Json };
        
        private static readonly Regex LinkPattern = new Regex("\\<(?<Uri>[^>]+)\\>;(.*rel=\"?(?<Relation>[^ \";]+)\"?;.*type=\"?(?<Type>[^ \";]+)\"?.*|.*type=\"?(?<Type>[^ \";]+)\"?;.*rel=\"?(?<Relation>[^ \";]+)\"?.*)");
        
        private static readonly Func<IHeaders, bool>[][] ExactMatchCases =
        {
            new Func<IHeaders, bool>[] { headers => headers["Content-Type"].Any(_ => _.Contains(JsonLd)) },
            new Func<IHeaders, bool>[]
            {
                headers => headers["Content-Type"].Any(_ => _.Contains(Json)),
                headers => { return headers["Link"].Any(_ => _.Contains(JsonLdContext) && _.Contains(JsonLd)); }
            }
        };

        /// <summary>Initializes a new instance of the <see cref="JsonLdHypermediaProcessor" /> class.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        /// <param name="httpCall">HTTP call facility.</param>
        public JsonLdHypermediaProcessor(IOntologyProvider ontologyProvider, HttpCallFacility httpCall)
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
                foreach (var approach in ExactMatchCases)
                {
                    var currentMatch = approach.All(_ => _(response.Headers));
                    if (currentMatch)
                    {
                        result = Level.FullSupport;
                        break;
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        protected override async Task<IRdfReader> CreateRdfReader(IResponse response, CancellationToken cancellationToken)
        {
            return new JsonLdReader(await TryObtainJsonLdContext(response, cancellationToken), _ => LoadDocument(_, cancellationToken));
        }

        /// <inheritdoc />
        protected override IRdfWriter CreateRdfWriter()
        {
            return new JsonLdWriter();
        }

        private static Tuple<Uri, string> GetJsonLdContextUri(IResponse response)
        {
            return (
                from item in response.Headers["Link"]
                let pattern = LinkPattern.Match(item)
                where pattern.Success && pattern.Groups["Relation"].Value == JsonLdContext
                select new Tuple<Uri, string>(new Uri(response.Url, pattern.Groups["Uri"].Value), pattern.Groups["Type"].Value))
                .FirstOrDefault();
        }

        private async Task<JToken> TryObtainJsonLdContext(IResponse response, CancellationToken cancellationToken)
        {
            JToken result = null;
            Tuple<Uri, string> link;
            if (response.Headers["Content-Type"].Contains(Json) && (link = GetJsonLdContextUri(response)) != null)
            {
                var contextResponse = await HttpCall(link.Item1, new HttpOptions() { Headers = { { "Accept", link.Item2 } } }, cancellationToken);
                result = (await LoadDocument(contextResponse, cancellationToken))?.Document;
            }

            return result;
        }

        private async Task<JsonLdDocument> LoadDocument(IResponse response, CancellationToken cancellationToken)
        {
            JsonLdDocument result = null;
            if (response.Headers["Content-Type"].Any(_ => SupportedMediaTypes.Contains(_)))
            {
                using (var reader = new StreamReader(await response.GetBody(cancellationToken)))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    result = new JsonLdDocument()
                    {
                        Document = (JToken)JsonSerializer.CreateDefault().Deserialize(jsonReader),
                        DocumentUri = response.Url,
                        ContextUri = GetJsonLdContextUri(response)?.Item1
                    };
                }
            }

            return result;
        }

        private JsonLdDocument LoadDocument(Uri uri, CancellationToken cancellationToken)
        {
            var result = HttpCall(uri, null, CancellationToken.None).Result;
            return LoadDocument(result, cancellationToken).Result;
        }
    }
}
