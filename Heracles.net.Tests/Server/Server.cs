using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Heracles.Server
{
    public class Server
    {
        internal const int ServerPort = 3001;

        private static readonly string RootDirectory = Path.Combine(
            Path.GetDirectoryName(typeof(Server).Assembly.CodeBase.Replace("file:///", String.Empty).Replace('/', Path.DirectorySeparatorChar)),
            nameof(Server));

        private static readonly IDictionary<string, string> Extensions = new Dictionary<string, string>()
        {
            { ".jsonld", "application/ld+json" },
            { ".ttl", "text/turtle" }
        };

        private static JsonSerializer Json = JsonSerializer.CreateDefault();

        private readonly HttpListener _listener;
        private TaskCompletionSource<bool> _task;
        private IDisposable _registration;

        public Server()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{ServerPort}/");
        }

        public Task Start(CancellationToken cancellationToken)
        {
            if (_task == null)
            {
                _task = new TaskCompletionSource<bool>();
                _registration = cancellationToken.Register(Stop);
                Listen(cancellationToken);
                Console.WriteLine($"Hydra tests server is listening on port {ServerPort}...");
            }

            return _task.Task;
        }

        private async void Listen(CancellationToken cancellationToken)
        {
            _listener.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext context = null;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch
                {
                }

                if (context != null)
                {
                    switch (context.Request.HttpMethod)
                    {
                        case "OPTIONS":
                            SetCorsHeader(context.Response);
                            context.Response.StatusCode = 200;
                            break;
                        case "GET":
                            var path = context.Request.Url.AbsolutePath == "/" ? "/root" : context.Request.Url.AbsolutePath;
                            var output = LoadBody(context.Request.Url, path, context.Request.Url.Query);
                            if (SetHeaders(path, context.Response, output.MediaType) || output.Body != null)
                            {
                                context.Response.StatusCode = 200;
                                output.Body?.CopyTo(context.Response.OutputStream);
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                            }

                            break;
                        case "POST":
                            SetCorsHeader(context.Response);
                            var hash = MD5.Create().ComputeHash(context.Request.InputStream);
                            context.Response.Headers["Location"] = context.Request.Url + "/" + Convert.ToBase64String(hash);
                            context.Response.StatusCode = 201;
                            break;
                        case "PUT":
                            SetCorsHeader(context.Response);
                            context.Response.StatusCode = 201;
                            break;
                    }

                    context.Response.Close();
                }
            }
        }

        private static void SetCorsHeader(HttpListenerResponse response)
        {
            response.Headers["Access-Control-Allow-Origin"] = "*";
            response.Headers["Access-Control-Allow-Methods"] = "HEAD, GET, PUT, DELETE, POST";
            response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            response.Headers["Access-Control-Expose-Headers"] = "Link, Content-Type, Location";
        }

        private static bool SetHeaders(string path, HttpListenerResponse response, string mediaType)
        {
            response.Headers["Content-Type"] = mediaType;
            SetCorsHeader(response);
            var file = Path.Combine(RootDirectory, path.TrimStart('/') + ".headers");
            if (!File.Exists(file))
            {
                return false;
            }

            using (var stream = File.OpenText(file))
            {
                foreach (var header in stream.ReadToEnd().Replace("\r", String.Empty).Split('\n').Where(header => header.Length > 0))
                {
                    var name = header.Split(':')[0];
                    var value = header.Substring(name.Length + 1).Trim();
                    response.Headers[name] = value;
                }
            }

            return true;
        }

        private static Output LoadBody(Uri baseUri, string path, string query)
        {
            foreach (var extension in Extensions)
            {
                var file = Path.Combine(RootDirectory, path.TrimStart('/') + (path.IndexOf(".") == -1 ? extension.Key : String.Empty));
                if (File.Exists(file))
                {
                    Stream result;
                    using (var fileStream = File.Open(file, FileMode.Open))
                    {
                        if (extension.Key == ".jsonld")
                        {
                            result = ProcessJsonLdBody(baseUri, path, query, fileStream);
                        }
                        else if (extension.Key == ".ttl")
                        {
                            var buffer = new MemoryStream();
                            var data = $"@BASE <{baseUri}> .\r\n" + new StreamReader(fileStream).ReadToEnd();
                            buffer.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
                            result = buffer;
                        }
                        else
                        {
                            var buffer = new MemoryStream();
                            fileStream.CopyTo(buffer);
                            result = buffer;
                        }
                    }

                    result.Seek(0, SeekOrigin.Begin);
                    return new Output() { Body = result, MediaType = extension.Value };
                }
            }

            return new Output() { MediaType = "text/plain" };
        }

        private static Stream ProcessJsonLdBody(Uri baseUri, string path, string query, FileStream fileStream)
        {
            Stream result;
            using (var fileReader = new StreamReader(fileStream))
            using (var jsonReader = new JsonTextReader(fileReader))
            {
                var resource = (JObject)Json.Deserialize(jsonReader);
                var context = resource["@context"];
                if (context != null && context.Type == JTokenType.String)
                {
                    resource["@context"] = new Uri(baseUri, (string)context).ToString();
                }

                JObject matchingResource = resource;
                if (!String.IsNullOrEmpty(query))
                {
                    if (resource["@graph"] != null)
                    {
                        matchingResource = resource["@graph"].AsJEnumerable().OfType<JObject>().FirstOrDefault(_ => (string)_["@id"] == path);
                        matchingResource["@id"] = path + query;
                        matchingResource = new JObject(new JProperty("@graph", matchingResource));
                        matchingResource["@context"] = resource["@context"];
                    }
                }

                result = new MemoryStream();
                using (var fileWriter = new StreamWriter(result, Encoding.UTF8, 4096, true))
                using (var jsonWriter = new JsonTextWriter(fileWriter))
                {
                    Json.Serialize(jsonWriter, matchingResource);
                }
            }

            return result;
        }

        private void Stop()
        {
            _listener.Stop();
            _registration.Dispose();
        }

        ~Server()
        {
            _listener.Stop();
        }

        private class Output
        {
            internal string MediaType { get; set; }

            internal Stream Body { get; set; }
        }
    }
}
