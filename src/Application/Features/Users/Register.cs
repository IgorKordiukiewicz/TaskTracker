using Application.Data;
using Shared.Dtos;
using Domain;
using FluentValidation;
using MediatR;

namespace Application.Features.Users;

public record RegisterUserCommand(UserRegistrationDto Model) : IRequest;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Model.AuthenticationId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
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
        var user = User.Create(request.Model.AuthenticationId, request.Model.Name);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }
}
