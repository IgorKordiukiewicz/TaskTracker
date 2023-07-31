using Application.Errors;
using Application.Features.Organizations;
using Application.Features.Users;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using System.Security.Claims;

namespace Web.Server;

public static class Endpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("users/is-registered/{userAuthenticationId}", async (IMediator mediator, string userAuthenticationId) =>
        {
            var result = await mediator.Send(new IsUserRegisteredQuery(userAuthenticationId));
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapPost("users/register", async (IMediator mediator, [FromBody] UserRegistrationDto model) =>
        {
            var result = await mediator.Send(new RegisterUserCommand(model));
            return result.ToHttpResult(201);
        }).RequireAuthorization();

        app.MapGet("users/not-in-org", async (IMediator mediator, Guid organizationId, string searchValue) =>
        {
            var result = await mediator.Send(new GetUsersNotInOrganizationQuery(organizationId, searchValue));
            return result.ToHttpResult();
        });

        app.MapPost("organizations", async (IMediator mediator, [FromBody] CreateOrganizationDto model) =>
        {
            var result = await mediator.Send(new CreateOrganizationCommand(model));
            return result.ToHttpResult(); // TODO: Return 201 with ID
        }).RequireAuthorization();

        app.MapGet("organizations", async ([FromServices] IMediator mediator, [FromServices] IHttpContextAccessor contextAccessor) =>
        {
            var userAuthenticationId = contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await mediator.Send(new GetOrganizationsForUserQuery(userAuthenticationId));
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
