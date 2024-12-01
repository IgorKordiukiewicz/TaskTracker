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

internal class DeleteProjectRoleHandler : IRequestHandler<DeleteOrganizationRoleCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public DeleteProjectRoleHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(DeleteOrganizationRoleCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.DeleteRole(request.Model.RoleId, organization.Members);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}