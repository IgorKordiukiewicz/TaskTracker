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

internal class AcceptOrganizationInvitationHandler : IRequestHandler<AcceptOrganizationInvitationCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public AcceptOrganizationInvitationHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }


    public async Task<Result> Handle(AcceptOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>($"invitation ID: {request.InvitationId}"));
        }

        var result = organization.AcceptInvitation(request.InvitationId);

        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
