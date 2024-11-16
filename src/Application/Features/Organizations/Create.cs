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
        if (!await _context.Users.AnyAsync(x => x.Id == request.OwnerId))
        {
            return Result.Fail(new NotFoundError<User>(request.OwnerId));
        }

        var organization = Organization.Create(request.Model.Name, request.OwnerId);

        await _organizationRepository.Add(organization);

        return organization.Id;
    }
}
