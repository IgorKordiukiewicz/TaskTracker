using Application.Data.Repositories;
using Application.Errors;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record UpdateOrganizationMemberRoleCommand(Guid OrganizationId, Guid MemberId, UpdateMemberRoleDto Model) : IRequest<Result>;

internal class UpdateOrganizationMemberRoleCommandValidator : AbstractValidator<UpdateOrganizationMemberRoleCommand>
{
    public UpdateOrganizationMemberRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
    }
}

internal class UpdateOrganizationMemberRoleHandler : IRequestHandler<UpdateOrganizationMemberRoleCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public UpdateOrganizationMemberRoleHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(UpdateOrganizationMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.UpdateMemberRole(request.MemberId, request.Model.RoleId, organization.Members);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization);
        return Result.Ok();
    }
}