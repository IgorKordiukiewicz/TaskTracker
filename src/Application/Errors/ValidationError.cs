using FluentResults;
using FluentValidation.Results;

namespace Application.Errors;

public class ValidationError : ApplicationError
{
    public ValidationError(IEnumerable<ValidationFailure> validationFailures)
        : base(CreateMessage(validationFailures)) { }

    private static string CreateMessage(IEnumerable<ValidationFailure> validationFailures)
    {
        var errorMessages = validationFailures.Select(x => x.ErrorMessage);
        return string.Join(";", errorMessages);
    }
}
