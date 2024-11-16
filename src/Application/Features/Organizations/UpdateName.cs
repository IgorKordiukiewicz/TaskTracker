
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

internal class UpdateOrganizationNameHandler : IRequestHandler<UpdateOrganizationNameCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;

    public UpdateOrganizationNameHandler(IRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(UpdateOrganizationNameCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId);
        if(organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        organization.Name = request.Model.Name;
        await _organizationRepository.Update(organization);

        return Result.Ok();
    }
}
