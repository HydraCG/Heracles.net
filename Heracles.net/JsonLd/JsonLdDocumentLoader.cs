using System;
using System.IO;
using System.Threading;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Heracles.JsonLd
{
    internal class JsonLdDocumentLoader : DocumentLoader
    {
        private readonly HttpCallFacility _httpCall;

        internal JsonLdDocumentLoader(HttpCallFacility httpCall)
        {
            _httpCall = httpCall;
        }

        /// <inheritdoc />
        public override RemoteDocument LoadDocument(string url)
        {
            var result = _httpCall(new Uri(url), null, CancellationToken.None).Result.GetBody().Result;
            using (var reader = new StreamReader(result))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return new RemoteDocument(url, (JToken)JsonSerializer.CreateDefault().Deserialize(jsonReader));
            }
        }
    }
}
