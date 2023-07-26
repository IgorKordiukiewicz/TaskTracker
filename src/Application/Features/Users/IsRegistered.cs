using Application.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users;

public record IsUserRegisteredQuery(string AuthenticationId) : IRequest<bool>;

internal class IsUserRegisteredQueryValidator : AbstractValidator<IsUserRegisteredQuery>
{
    public IsUserRegisteredQueryValidator()
    {
        RuleFor(x => x.AuthenticationId).NotEmpty();
    }
}

internal class IsUserRegisteredHandler : IRequestHandler<IsUserRegisteredQuery, bool>
{
    private readonly AppDbContext _dbContext;

    public IsUserRegisteredHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(IsUserRegisteredQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(x => x.AuthenticationId == request.AuthenticationId);
    }
}
