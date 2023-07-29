using Application.Data;
using Shared.Dtos;
using FluentValidation;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Domain.Users;

namespace Application.Features.Users;

public record RegisterUserCommand(UserRegistrationDto Model) : IRequest<Result>;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Model.AuthenticationId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
    }
}

internal class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
{
    private readonly AppDbContext _dbContext; // TODO: Use repositories for write-side

    public RegisterUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if(await _dbContext.Users.AnyAsync(x => x.AuthenticationId == request.Model.AuthenticationId))
        {
            return Result.Fail(new Error("User is already registered in the database."));
        }

        var user = User.Create(request.Model.AuthenticationId, request.Model.Name);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}
