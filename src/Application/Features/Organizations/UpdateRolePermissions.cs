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

internal class UpdateProjectRolePermissionsHandler : IRequestHandler<UpdateOrganizationRolePermissionsCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public UpdateProjectRolePermissionsHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(UpdateOrganizationRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.UpdateRolePermissions(request.Model.RoleId, request.Model.Permissions);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
