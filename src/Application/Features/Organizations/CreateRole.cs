using Domain.Organizations;

namespace Application.Features.Organizations;

public record CreateOrganizationRoleCommand(Guid OrganizationId, CreateRoleDto<OrganizationPermissions> Model) : IRequest<Result>;

internal class CreateOrganizationRoleCommandValidator : AbstractValidator<CreateOrganizationRoleCommand>
{
    public CreateOrganizationRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
        RuleFor(x => x.Model.Permissions).NotNull(); // TODO: Validate in range
    }
}

internal class CreateOrganizationRoleHandler : IRequestHandler<CreateOrganizationRoleCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public CreateOrganizationRoleHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(CreateOrganizationRoleCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.AddRole(request.Model.Name, request.Model.Permissions);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
