using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Heracles.Namespaces;

namespace Heracles
{
    /// <summary>Exception raised when no <see cref="hydra.entrypoint" /> is defined.</summary>
    [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
    [ExcludeFromCodeCoverage]
    public class NoEntrypointDefinedException : HydraClientException
    {
        private const string DefaultMessage = "API documentation has no entry point defined.";
        
        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        public NoEntrypointDefinedException() : base(DefaultMessage)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        public NoEntrypointDefinedException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public NoEntrypointDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected NoEntrypointDefinedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}