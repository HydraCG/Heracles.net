using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Heracles.Namespaces;

namespace Heracles
{
    /// <summary>Exception raised when no <see cref="hydra.ApiDocumentation" /> is defined.</summary>
    [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
    [ExcludeFromCodeCoverage]
    public class ApiDocumentationNotProvidedException : HydraClientException
    {
        private const string DefaultMessage = "API documentation not provided.";
        
        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        public ApiDocumentationNotProvidedException() : base(DefaultMessage)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        public ApiDocumentationNotProvidedException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public ApiDocumentationNotProvidedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NoEntrypointDefinedException" /> class.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected ApiDocumentationNotProvidedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}