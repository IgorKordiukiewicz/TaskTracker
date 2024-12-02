using Domain.Organizations;

namespace Application.Features.Organizations;

public record DeclineOrganizationInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class DeclineOrganizationInvitationCommandValidator : AbstractValidator<DeclineOrganizationInvitationCommand>
{
    public DeclineOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class DeclineOrganizationInvitationHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<DeclineOrganizationInvitationCommand, Result>
{
    public async Task<Result> Handle(DeclineOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if(organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>($"invitation ID: {request.InvitationId}"));
        }

        var result = organization.DeclineInvitation(request.InvitationId);

        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
