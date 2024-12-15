using Domain.Organizations;
using Domain.Users;

namespace Application.Features.Organizations;

public record CreateOrganizationCommand(CreateOrganizationDto Model, Guid OwnerId) : IRequest<Result<Guid>>;

internal class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
    }
}

internal class CreateOrganizationHandler(AppDbContext context, IRepository<Organization> organizationRepository) 
    : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if(await organizationRepository.Exists(x => x.Name == request.Model.Name, cancellationToken))
        {
            return Result.Fail<Guid>(new ApplicationError("Organization with the same name already exists."));
        }

        var organization = Organization.Create(request.Model.Name, request.OwnerId);

        await organizationRepository.Add(organization, cancellationToken);

        return organization.Id;
    }
}
