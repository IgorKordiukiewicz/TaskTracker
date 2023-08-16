using Application.Data.Repositories;
using Application.Errors;
using Domain.Organizations;

namespace Application.Features.Organizations;

// TODO: Create Invitations folder: Features/Organizations/Invitations
public record DeclineOrganizationInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class DeclineOrganizationInvitationCommandValidator : AbstractValidator<DeclineOrganizationInvitationCommand>
{
    public DeclineOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class DeclineOrganizationInvitationHandler : IRequestHandler<DeclineOrganizationInvitationCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public DeclineOrganizationInvitationHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(DeclineOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId));
        if(organization is null)
        {
            return Result.Fail(new ApplicationError("Organization with this invitation does not exist."));
        }

        var result = organization.DeclineInvitation(request.InvitationId);

        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization);

        return Result.Ok();
    }
}
