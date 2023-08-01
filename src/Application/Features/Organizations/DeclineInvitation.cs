namespace Application.Features.Organizations;

// TODO: Create Invitations folder: Features/Organizations/Invitations
public record DeclineOrganizationInvitationCommand(DeclineOrganizationInvitationDto Model) : IRequest<Result>;

internal class DeclineOrganizationInvitationCommandValidator : AbstractValidator<DeclineOrganizationInvitationCommand>
{
    public DeclineOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.Model.InvitationId).NotEmpty();
    }
}

internal class DeclineOrganizationInvitationHandler : IRequestHandler<DeclineOrganizationInvitationCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public DeclineOrganizationInvitationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeclineOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _dbContext.Organizations
            .Include(x => x.Invitations)
            .Where(x => x.Invitations.Any(xx => xx.Id == request.Model.InvitationId))
            .FirstOrDefaultAsync();

        if(organization is null)
        {
            return Result.Fail(new Error("Organization with this invitation does not exist."));
        }

        var result = organization.DeclineInvitation(request.Model.InvitationId);

        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _dbContext.SaveChangesAsync();
        return Result.Ok();
    }
}
