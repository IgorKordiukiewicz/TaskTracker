namespace Application.Features.Organizations;

public record AcceptOrganizationInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class AcceptOrganizationInvitationCommandValidator : AbstractValidator<AcceptOrganizationInvitationCommand>
{
    public AcceptOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class AcceptOrganizationInvitationHandler : IRequestHandler<AcceptOrganizationInvitationCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public AcceptOrganizationInvitationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AcceptOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _dbContext.Organizations
            .Include(x => x.Invitations)
            .Where(x => x.Invitations.Any(xx => xx.Id == request.InvitationId))
            .FirstOrDefaultAsync();

        if (organization is null)
        {
            return Result.Fail(new Error("Organization with this invitation does not exist."));
        }

        var result = organization.AcceptInvitation(request.InvitationId);

        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _dbContext.OrganizationMembers.AddAsync(result.Value); // TODO: has to be manually added because Members are not included, so Include members and this line can be removed
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}
