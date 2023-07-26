using Application.Features.Users;
using MediatR;

namespace Web.Server;

public static class Endpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("users/try-register/{userAuthenticationId}", async (IMediator mediator, string userAuthenticationId) =>
        {
            if (!await mediator.Send(new IsUserRegisteredQuery(userAuthenticationId)))
            {
                await mediator.Send(new RegisterUserCommand(userAuthenticationId));
                return Results.StatusCode(201);
            }

            return Results.Ok();
        }).RequireAuthorization();
    }
}
