using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;
using Moq;

namespace Heracles.Testing
{
    public static class Return
    {
        private static readonly IDictionary<string, IEnumerable<string>> DefaultHeaders =
            new Dictionary<string, IEnumerable<string>>()
            {
                { "Content-Type", new[] { "application/ld+json" } }
            };

        public static IResponse Ok(
            Uri url = null,
            Stream body = null,
            IDictionary<string, IEnumerable<string>> headers = null,
            int status = 200)
        {
            return ReturnResponse(url, body, headers, status);
        }

        public static IResponse NotFound(
            Uri url = null,
            Stream body = null,
            IDictionary<string, IEnumerable<string>> headers = null,
            int status = 404)
        {
            return ReturnResponse(url, body, headers, status);
        }

        private static IResponse ReturnResponse(
            Uri url,
            Stream body = null,
            IDictionary<string, IEnumerable<string>> headers = null,
            int status = 200)
        {
            var responseHeaders = new Mock<IHeaders>(MockBehavior.Strict);
            IEnumerable<string> header;
            responseHeaders.Setup(_ => _[It.IsAny<string>()])
                .Returns<string>(_ => (headers ?? DefaultHeaders).TryGetValue(_, out header) ? header : Array.Empty<string>());
            var result = new Mock<IResponse>(MockBehavior.Strict);
            result.SetupGet(_ => _.Url).Returns(url);
            result.SetupGet(_ => _.Status).Returns(status);
            result.SetupGet(_ => _.Headers).Returns(responseHeaders.Object);
            result.Setup(_ => _.GetBody(It.IsAny<CancellationToken>())).Returns(Task.FromResult(body));
            return result.Object;
        }
    }
}
