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

internal class CreateOrganizationInvitationHandler(AppDbContext dbContext, IRepository<Organization> organizationRepository) 
    : IRequestHandler<CreateOrganizationInvitationCommand, Result>
{
    public async Task<Result> Handle(CreateOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        if (!await dbContext.Users.AnyAsync(x => x.Id == request.Model.UserId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<User>(request.Model.UserId));
        }

        var invitationResult = organization.CreateInvitation(request.Model.UserId);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }
        
        await organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
