using Domain.Organizations;

namespace Application.Features.Organizations;

public record AcceptOrganizationInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class AcceptOrganizationInvitationCommandValidator : AbstractValidator<AcceptOrganizationInvitationCommand>
{
    public AcceptOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class AcceptOrganizationInvitationHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<AcceptOrganizationInvitationCommand, Result>
{
    public async Task<Result> Handle(AcceptOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>($"invitation ID: {request.InvitationId}"));
        }

        var result = organization.AcceptInvitation(request.InvitationId);

        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
