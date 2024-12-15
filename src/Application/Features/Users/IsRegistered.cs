namespace Application.Features.Users;

public record IsUserRegisteredQuery(Guid Id) : IRequest<Result<bool>>;

internal class IsUserRegisteredQueryValidator : AbstractValidator<IsUserRegisteredQuery>
{
    public IsUserRegisteredQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal class IsUserRegisteredHandler(AppDbContext dbContext) 
    : IRequestHandler<IsUserRegisteredQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsUserRegisteredQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Users.AnyAsync(x => x.Id == request.Id, cancellationToken);
    }
}
