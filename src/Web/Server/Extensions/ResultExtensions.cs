﻿using Application.Errors;
using Domain.Errors;
using FluentResults;
using Infrastructure.Errors;

namespace Web.Server.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToHttpResult<T>(this Result<T> requestResult)
    {
        if (!requestResult.IsFailed)
        {
            return new OkObjectResult(requestResult.Value);
        }

        return GetErrorResult(requestResult.Errors);
    }

    public static IActionResult ToHttpResult(this Result requestResult, int? statusCode = null)
    {
        if (!requestResult.IsFailed)
        {
            return statusCode is not null
                ? new StatusCodeResult(statusCode.Value)
                : new OkResult();
        }

        return GetErrorResult(requestResult.Errors);
    }

    private static IActionResult GetErrorResult(List<IError> errors)
    {
        var error = errors.FirstOrDefault();
        return error switch
        {
            { } e when e.GetType().IsGenericType && e.GetType().GetGenericTypeDefinition() == typeof(NotFoundError<>) => new NotFoundObjectResult(error.Message),
            InfrastructureError => new StatusCodeResult(500),
            ApplicationError => new BadRequestObjectResult(error.Message),
            DomainError => new BadRequestObjectResult(error.Message),
            _ => new StatusCodeResult(500)
        };
    }
}
