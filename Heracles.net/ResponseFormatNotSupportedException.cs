using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Heracles
{
    /// <summary>Exception raised when a response of an unsupported format was received.</summary>
    [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
    [ExcludeFromCodeCoverage]
    public class ResponseFormatNotSupportedException : HydraClientException
    {
        private const string DefaultMessage = "Response format is not supported.";

        /// <summary>Initializes a new instance of the <see cref="ResponseFormatNotSupportedException" /> class.</summary>
        public ResponseFormatNotSupportedException() : base(DefaultMessage)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseFormatNotSupportedException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        public ResponseFormatNotSupportedException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseFormatNotSupportedException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public ResponseFormatNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ResponseFormatNotSupportedException" /> class.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected ResponseFormatNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}