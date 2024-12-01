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

internal class CancelOrganizationInvitationHandler : IRequestHandler<CancelOrganizationInvitationCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public CancelOrganizationInvitationHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(CancelOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>($"invitation ID: {request.InvitationId}"));
        }

        var result = organization.CancelInvitation(request.InvitationId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
