using Domain.Organizations;

namespace Application.Features.Organizations;

public record CancelOrganizationInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class CancelOrganizationInvitationCommandValidator : AbstractValidator<CancelOrganizationInvitationCommand>
{
    public CancelOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class CancelOrganizationInvitationHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<CancelOrganizationInvitationCommand, Result>
{
    public async Task<Result> Handle(CancelOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>($"invitation ID: {request.InvitationId}"));
        }

        var result = organization.CancelInvitation(request.InvitationId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
