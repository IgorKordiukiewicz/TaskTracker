using Domain.Organizations;

namespace Application.Features.Organizations;

public record UpdateOrganizationRoleNameCommand(Guid OrganizationId, UpdateRoleNameDto Model) : IRequest<Result>;

internal class UpdateOrganizationRoleNameCommandValidator : AbstractValidator<UpdateOrganizationRoleNameCommand>
{
    public UpdateOrganizationRoleNameCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
    }
}

internal class UpdateOrganizationRoleNameHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<UpdateOrganizationRoleNameCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationRoleNameCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.UpdateRoleName(request.Model.RoleId, request.Model.Name);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
