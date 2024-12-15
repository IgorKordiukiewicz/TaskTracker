using Domain.Organizations;

namespace Application.Features.Organizations;

public record UpdateOrganizationMemberRoleCommand(Guid OrganizationId, UpdateMemberRoleDto Model) : IRequest<Result>;

internal class UpdateOrganizationMemberRoleCommandValidator : AbstractValidator<UpdateOrganizationMemberRoleCommand>
{
    public UpdateOrganizationMemberRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.MemberId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
    }
}

internal class UpdateOrganizationMemberRoleHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<UpdateOrganizationMemberRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.UpdateMemberRole(request.Model.MemberId, request.Model.RoleId, organization.Members);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}