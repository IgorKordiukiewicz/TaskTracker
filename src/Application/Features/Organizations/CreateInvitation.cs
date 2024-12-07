using Application.Common;
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
        RuleFor(x => x.Model.ExpirationDays).Must(x => x > 0).When(x => x.Model.ExpirationDays is not null);
    }
}

internal class CreateOrganizationInvitationHandler(AppDbContext dbContext, IRepository<Organization> organizationRepository, IDateTimeProvider dateTimeProvider) 
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

        var invitationResult = organization.CreateInvitation(request.Model.UserId, dateTimeProvider.Now(), request.Model.ExpirationDays);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }
        
        await organizationRepository.Update(organization, cancellationToken);

        return Result.Ok();
    }
}
