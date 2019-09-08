using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Heracles
{
    /// <summary>Provides a general <see cref="HydraClient" /> exception.</summary>
    [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
    [ExcludeFromCodeCoverage]
    public class HydraClientException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="HydraClientException" /> class.</summary>
        public HydraClientException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HydraClientException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        public HydraClientException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HydraClientException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public HydraClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HydraClientException" /> class.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected HydraClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}