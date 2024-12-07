using Application.Common;
using Application.Features.Projects;
using Domain.Notifications;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record RemoveOrganizationMemberCommand(Guid OrganizationId, RemoveOrganizationMemberDto Model) : IRequest<Result>;

internal class RemoveOrganizationMemberCommandValidator : AbstractValidator<RemoveOrganizationMemberCommand>
{
    public RemoveOrganizationMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.MemberId).NotEmpty();
    }
}

internal class RemoveOrganizationMemberHandler(IRepository<Organization> organizationRepository, AppDbContext dbContext, 
    IMediator mediator, IJobsService jobsService, IDateTimeProvider dateTimeProvider) 
    : IRequestHandler<RemoveOrganizationMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveOrganizationMemberCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var userId = organization.Members.FirstOrDefault(x => x.Id == request.Model.MemberId)?.UserId;

        var result = organization.RemoveMember(request.Model.MemberId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        // TODO: use transaction (can't wrap in transaction when using the remove project member command - nested transaction)
        await organizationRepository.Update(organization, cancellationToken);

        var projectsMembers = await dbContext.Projects
            .Where(x => x.OrganizationId == organization.Id && x.Members.Any(xx => xx.UserId == userId))
            .Select(v => new { ProjectId = v.Id, MemberId = v.Members.First(x => x.UserId == userId).Id })
            .ToListAsync(cancellationToken);

        foreach (var projectMember in projectsMembers)
        {
            var projectHandlerResult = await mediator.Send(new RemoveProjectMemberCommand(projectMember.ProjectId, new(projectMember.MemberId)), cancellationToken);
            if (projectHandlerResult.IsFailed)
            {
                return Result.Fail(projectHandlerResult.Errors);
            }
        }

        jobsService.EnqueueCreateNotification(NotificationFactory.RemovedFromOrganization(userId!.Value, dateTimeProvider.Now(), organization.Id));

        return Result.Ok();
    }
}
