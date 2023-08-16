using Application.Data.Repositories;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record RemoveOrganizationMemberCommand(Guid OrganizationId, Guid MemberId) : IRequest<Result>;

internal class RemoveOrganizationMemberCommandValidator : AbstractValidator<RemoveOrganizationMemberCommand>
{
    public RemoveOrganizationMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
    }
}

internal class RemoveOrganizationMemberHandler : IRequestHandler<RemoveOrganizationMemberCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public RemoveOrganizationMemberHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(RemoveOrganizationMemberCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId);
        if (organization is null)
        {
            return Result.Fail(new Error("Organization with this ID does not exist."));
        }

        var result = organization.RemoveMember(request.MemberId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization);

        return Result.Ok();
    }
}
