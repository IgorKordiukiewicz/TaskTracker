namespace Application.Features.Users;

public record IsUserRegisteredQuery(Guid Id) : IRequest<Result<bool>>;

internal class IsUserRegisteredQueryValidator : AbstractValidator<IsUserRegisteredQuery>
{
    public IsUserRegisteredQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal class IsUserRegisteredHandler : IRequestHandler<IsUserRegisteredQuery, Result<bool>>
{
    private readonly AppDbContext _dbContext;

    public IsUserRegisteredHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(IsUserRegisteredQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(x => x.Id == request.Id, cancellationToken);
    }
}
