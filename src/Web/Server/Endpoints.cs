using Application.Errors;
using Application.Features.Organizations;
using Application.Features.Projects;
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
        app.MapGroup("users").AddUsersEndpoints();
        app.MapGroup("organizations").AddOrganizationsEndpoints();
        app.MapGroup("projects").AddProjectsEndpoints();
    }

    private static void AddUsersEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("is-registered/{userAuthenticationId}", async (IMediator mediator, string userAuthenticationId) =>
        {
            var result = await mediator.Send(new IsUserRegisteredQuery(userAuthenticationId));
            return result.ToHttpResult();
        }).RequireAuthorization();

        builder.MapPost("register", async (IMediator mediator, [FromBody] UserRegistrationDto model) =>
        {
            var result = await mediator.Send(new RegisterUserCommand(model));
            return result.ToHttpResult(201);
        }).RequireAuthorization();

        builder.MapGet("not-in-org", async (IMediator mediator, Guid organizationId, string searchValue) =>
        {
            var result = await mediator.Send(new GetUsersNotInOrganizationQuery(organizationId, searchValue));
            return result.ToHttpResult();
        }).RequireAuthorization();
    }

    private static void AddOrganizationsEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", async (IMediator mediator, [FromBody] CreateOrganizationDto model) =>
        {
            var result = await mediator.Send(new CreateOrganizationCommand(model));
            return result.ToHttpResult(); // TODO: Return 201 with ID
        }).RequireAuthorization();

        builder.MapGet("/", async (IMediator mediator, ClaimsPrincipal claimsPrincipal) =>
        {
            var result = await mediator.Send(new GetOrganizationsForUserQuery(claimsPrincipal.GetUserAuthenticationId()));
            return result.ToHttpResult();
        }).RequireAuthorization();

        // TODO: just "invitations"
        builder.MapPost("invitations/create", async (IMediator mediator, [FromBody] CreateOrganizationInvitationDto model) =>
        {
            var result = await mediator.Send(new CreateOrganizationInvitationCommand(model));
            return result.ToHttpResult();
        }).RequireAuthorization();

        builder.MapGet("invitations/user", async (IMediator mediator, ClaimsPrincipal claimsPrincipal) =>
        {
            // TODO: Store userId in claims?
            var result = await mediator.Send(new GetOrganizationInvitationsForUserQuery(claimsPrincipal.GetUserAuthenticationId()));
            return result.ToHttpResult();
        }).RequireAuthorization();

        builder.MapPost("invitations/decline", async (IMediator mediator, [FromBody] UpdateOrganizationInvitationDto model) =>
        {
            var result = await mediator.Send(new DeclineOrganizationInvitationCommand(model));
            return result.ToHttpResult();
        }).RequireAuthorization();

        builder.MapPost("invitations/accept", async (IMediator mediator, [FromBody] UpdateOrganizationInvitationDto model) =>
        {
            var result = await mediator.Send(new AcceptOrganizationInvitationCommand(model));
            return result.ToHttpResult();
        }).RequireAuthorization();
    }

    private static void AddProjectsEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", async (IMediator mediator, [FromBody] CreateProjectDto model) =>
        {
            var result = await mediator.Send(new CreateProjectCommand(model));
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
