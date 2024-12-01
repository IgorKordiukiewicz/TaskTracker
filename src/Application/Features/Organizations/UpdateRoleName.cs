﻿using Domain.Organizations;

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

internal class UpdateOrganizationRoleNameHandler : IRequestHandler<UpdateOrganizationRoleNameCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public UpdateOrganizationRoleNameHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(UpdateOrganizationRoleNameCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.RolesManager.UpdateRoleName(request.Model.RoleId, request.Model.Name);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization, cancellationToken);
        return Result.Ok();
    }
}
