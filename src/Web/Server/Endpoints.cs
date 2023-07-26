using Application.Features.Users;
using Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Server;

public static class Endpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("users/try-register", async (IMediator mediator, [FromBody] UserRegistrationDto model) =>
        {
            if (!await mediator.Send(new IsUserRegisteredQuery(model.AuthenticationId)))
            {
                await mediator.Send(new RegisterUserCommand(model));
                return Results.StatusCode(201);
            }

            return Results.Ok();
        }).RequireAuthorization();
    }
}
