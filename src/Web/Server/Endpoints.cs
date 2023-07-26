using Application.Features.Users;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

namespace Web.Server;

public static class Endpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("users/is-registered/{userAuthenticationId}", async (IMediator mediator, string userAuthenticationId) =>
        {
            var result = await mediator.Send(new IsUserRegisteredQuery(userAuthenticationId));
            return result.ToHttpResult();
        });

        app.MapPost("users/register", async (IMediator mediator, [FromBody] UserRegistrationDto model) =>
        {
            var result = await mediator.Send(new RegisterUserCommand(model));
            return result.ToHttpResult();
        }).RequireAuthorization();
    }

    private static IResult ToHttpResult<T>(this Result<T> requestResult)
    {
        if(!requestResult.IsFailed)
        {
            return Results.Ok(requestResult.Value);
        }

        return GetErrorResult(requestResult.Errors);
    }

    private static IResult ToHttpResult(this Result requestResult, int? statusCode = null)
    {
        if(!requestResult.IsFailed)
        {
            return statusCode is not null
                ? Results.StatusCode(statusCode.Value)
                : Results.Ok();
        }

        return GetErrorResult(requestResult.Errors);
    }

    private static IResult GetErrorResult(List<IError> errors)
    {
        var error = errors.FirstOrDefault();
        return error switch
        {
            Error => Results.BadRequest(error.Message),
            _ => Results.StatusCode(500)
        };
    }
}
