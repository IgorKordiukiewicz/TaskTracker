using Domain.Organizations;

namespace Application.Features.Organizations;

public record DeleteOrganizationRoleCommand(Guid OrganizationId, DeleteRoleDto Model) : IRequest<Result>;

internal class DeleteOrganizationRoleCommandValidator : AbstractValidator<DeleteOrganizationRoleCommand>
{
    public DeleteOrganizationRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
    }
}

internal class DeleteProjectRoleHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<DeleteOrganizationRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationRoleCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.DeleteRole(request.Model.RoleId, organization.Members);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}