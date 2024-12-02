using Domain.Organizations;

namespace Application.Features.Organizations;

public record UpdateOrganizationRolePermissionsCommand(Guid OrganizationId, UpdateRolePermissionsDto<OrganizationPermissions> Model) : IRequest<Result>;

internal class UpdateOrganizationRolePermissionsCommandValidator : AbstractValidator<UpdateOrganizationRolePermissionsCommand>
{
    public UpdateOrganizationRolePermissionsCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
        RuleFor(x => x.Model.Permissions).NotEmpty();
    }
}

internal class UpdateProjectRolePermissionsHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<UpdateOrganizationRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.UpdateRolePermissions(request.Model.RoleId, request.Model.Permissions);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
