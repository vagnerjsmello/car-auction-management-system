using FluentValidation;
using FluentValidation.Results;

namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails for a command or query.
/// </summary>
public class ValidationFailedException : ValidationException
{
    private const string DefaultMessage = "Validation failed for the command.";

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationFailedException"/> class
    /// with a default message and the list of validation failures.
    /// </summary>
    /// <param name="failures">The list of validation failures.</param>
    public ValidationFailedException(IEnumerable<ValidationFailure> failures) : base(DefaultMessage, failures) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationFailedException"/> class
    /// with a custom message and the list of validation failures.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="failures">The list of validation failures.</param>
    public ValidationFailedException(string message, IEnumerable<ValidationFailure> failures) : base(message, failures) { }

    /// <summary>
    /// Gets the error message with appended validation errors.
    /// </summary>
    public override string Message
    {
        get
        {
            var errors = string.Join("; ", this.Errors.Select(e => e.ErrorMessage));
            return $"{base.Message} Errors: {errors}";
        }
    }
}
