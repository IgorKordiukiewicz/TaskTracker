using Domain.Organizations;
using Domain.Users;

namespace Application.Features.Organizations;

public record CreateOrganizationInvitationCommand(Guid OrganizationId, CreateOrganizationInvitationDto Model) : IRequest<Result>;

internal class CreateOrganizationInvitationCommandValidator : AbstractValidator<CreateOrganizationInvitationCommand>
{
    public CreateOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.UserId).NotEmpty();
    }
}

internal class CreateOrganizationInvitationHandler : IRequestHandler<CreateOrganizationInvitationCommand, Result>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Organization> _organizationRepository;

    public CreateOrganizationInvitationHandler(AppDbContext dbContext, IRepository<Organization> organizationRepository)
    {
        _dbContext = dbContext;
        _organizationRepository = organizationRepository;
    }

    public async Task<Result> Handle(CreateOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        if (!await _dbContext.Users.AnyAsync(x => x.Id == request.Model.UserId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<User>(request.Model.UserId));
        }

        var invitationResult = organization.CreateInvitation(request.Model.UserId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }
        
        await _organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
