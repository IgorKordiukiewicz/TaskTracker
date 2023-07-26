using Application.Data;
using Domain;
using FluentValidation;
using MediatR;

namespace Application.Features.Users;

public record RegisterUserCommand(string AuthenticationId) : IRequest;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.AuthenticationId).NotEmpty();
    }
}

internal class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly AppDbContext _dbContext; // TODO: Use repositories for write-side

    public RegisterUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.AuthenticationId);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }
}
