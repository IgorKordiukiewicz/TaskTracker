﻿using FluentValidation.Results;

namespace Application.Errors;

public class ValidationError(IEnumerable<ValidationFailure> validationFailures) 
    : ApplicationError(CreateMessage(validationFailures))
{
    private static string CreateMessage(IEnumerable<ValidationFailure> validationFailures)
    {
        var errorMessages = validationFailures.Select(x => x.ErrorMessage);
        return string.Join(";", errorMessages);
    }
}
