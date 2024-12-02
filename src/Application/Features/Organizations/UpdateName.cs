
using Domain.Organizations;

namespace Application.Features.Organizations;

public record UpdateOrganizationNameCommand(Guid OrganizationId, UpdateOrganizationNameDto Model) : IRequest<Result>;

internal class UpdateOrganizationNameCommandValidator : AbstractValidator<UpdateOrganizationNameCommand>
{
    public UpdateOrganizationNameCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
    }
}

internal class UpdateOrganizationNameHandler(IRepository<Organization> organizationRepository) 
    : IRequestHandler<UpdateOrganizationNameCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationNameCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if(organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        organization.Name = request.Model.Name;
        await organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
