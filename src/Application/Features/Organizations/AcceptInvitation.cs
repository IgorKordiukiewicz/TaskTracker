namespace Application.Features.Organizations;

public record AcceptOrganizationInvitationCommand(UpdateOrganizationInvitationDto Model) : IRequest<Result>;

internal class AcceptOrganizationInvitationCommandValidator : AbstractValidator<AcceptOrganizationInvitationCommand>
{
    public AcceptOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.Model.InvitationId).NotEmpty();
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
            .Where(x => x.Invitations.Any(xx => xx.Id == request.Model.InvitationId))
            .FirstOrDefaultAsync();

        if (organization is null)
        {
            return Result.Fail(new Error("Organization with this invitation does not exist."));
        }

        var result = organization.AcceptInvitation(request.Model.InvitationId);

        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _dbContext.OrganizationMembers.AddAsync(result.Value);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}
