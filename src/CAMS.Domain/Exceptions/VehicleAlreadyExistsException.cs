namespace CAMS.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle with the specified identifier already exists.
    /// </summary>
    public class VehicleAlreadyExistsException : Exception
    {
        private const string DefaultMessage = "A vehicle with the specified identifier already exists.";

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class with the default error message.
        /// </summary>
        public VehicleAlreadyExistsException() : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public VehicleAlreadyExistsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public VehicleAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
