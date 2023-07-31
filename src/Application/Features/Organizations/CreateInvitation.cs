using Domain.Organizations;

namespace Application.Features.Organizations;

public record CreateOrganizationInvitationCommand(CreateOrganizationInvitationDto Model) : IRequest<Result>;

internal class CreateOrganizationInvitationCommandValidator : AbstractValidator<CreateOrganizationInvitationCommand>
{
    public CreateOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.Model.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.UserId).NotEmpty();
    }
}

internal class CreateOrganizationInvitationHandler : IRequestHandler<CreateOrganizationInvitationCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public CreateOrganizationInvitationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(CreateOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var (organizationId, userId) = request.Model;

        var organization = await _dbContext.Organizations
            .Include(x => x.Invitations)
            .FirstOrDefaultAsync(x => x.Id == organizationId);
        if (organization is null)
        {
            return Result.Fail(new Error("Organization with this ID does not exist."));
        }

        if (!await _dbContext.Users.AnyAsync(x => x.Id == userId))
        {
            return Result.Fail(new Error("User with this ID does not exist."));
        }

        var invitationResult = organization.CreateInvitation(userId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }
        
        await _dbContext.OrganizationInvitations.AddAsync(invitationResult.Value);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}
