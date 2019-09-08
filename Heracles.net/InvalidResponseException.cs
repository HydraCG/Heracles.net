using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Heracles
{
    /// <summary>Exception raised when an invalid response was received.</summary>
    public class InvalidResponseException : HydraClientException
    {
        private const string DefaultMessage = "Remote server responded with a status.";
        private const string MessageFormat = "Remote server responded with a status of {0}.";

        /// <summary>Initializes a new instance of the <see cref="InvalidResponseException" /> class.</summary>
        [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
        [ExcludeFromCodeCoverage]
        public InvalidResponseException() : base(DefaultMessage)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InvalidResponseException" /> class.</summary>
        /// <param name="status">Response's status code.</param>
        [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
        [ExcludeFromCodeCoverage]
        public InvalidResponseException(int status) : base(String.Format(MessageFormat, status))
        {
            Status = status;
        }

        /// <summary>Initializes a new instance of the <see cref="InvalidResponseException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
        [ExcludeFromCodeCoverage]
        public InvalidResponseException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InvalidResponseException" /> class.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        [SuppressMessage("UnitTests", "TS000:NoUnitTests", Justification = "Nothing to test.")]
        [ExcludeFromCodeCoverage]
        public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InvalidResponseException" /> class.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected InvalidResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Status = info.GetInt32(nameof(Status));
        }
        
        /// <summary>Gets the response status that caused this exception.</summary>
        public int Status { get; }

        /// <summary>Obtains object data.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Status), Status);
        }
    }
}