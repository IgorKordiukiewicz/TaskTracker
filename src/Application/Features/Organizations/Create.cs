using Application.Data.Repositories;
using Application.Errors;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record CreateOrganizationCommand(CreateOrganizationDto Model) : IRequest<Result<Guid>>;

internal class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Model.OwnerId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
    }
}

internal class CreateOrganizationHandler : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    private readonly AppDbContext _context;
    private readonly IRepository<Organization> _organizationRepository;

    public CreateOrganizationHandler(AppDbContext context, IRepository<Organization> organizationRepository)
    {
        _context = context;
        _organizationRepository = organizationRepository;
    }

    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if(!await _context.Users.AnyAsync(x => x.Id == request.Model.OwnerId))
        {
            return Result.Fail(new ApplicationError("Owner does not exist"));
        }

        var organization = Organization.Create(request.Model.Name, request.Model.OwnerId);

        await _organizationRepository.Add(organization);

        return organization.Id;
    }
}
