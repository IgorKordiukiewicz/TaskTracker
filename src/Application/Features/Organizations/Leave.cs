using Application.Common;
using Application.Features.Projects;
using Domain.Notifications;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record LeaveOrganizationCommand(Guid OrganizationId, Guid UserId) : IRequest<Result>;

internal class LeaveOrganizationCommandValidator : AbstractValidator<LeaveOrganizationCommand>
{
    public LeaveOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class LeaveOrganizationHandler(IRepository<Organization> organizationRepository, IJobsService jobsService, 
    IDateTimeProvider dateTimeProvider, AppDbContext dbContext, IMediator mediator)
    : IRequestHandler<LeaveOrganizationCommand, Result>
{
    public async Task<Result> Handle(LeaveOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if(organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var result = organization.Leave(request.UserId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var userName = await dbContext.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => x.FullName)
            .FirstAsync(cancellationToken);

        var memberManagers = organization.GetMemberManagers();

        await organizationRepository.Update(organization, cancellationToken);

        // TODO: refactor
        var projectsMembers = await dbContext.Projects
            .Where(x => x.OrganizationId == organization.Id && x.Members.Any(xx => xx.UserId == request.UserId))
            .Select(v => new { ProjectId = v.Id, MemberId = v.Members.First(x => x.UserId == request.UserId).Id })
            .ToListAsync(cancellationToken);

        foreach (var projectMember in projectsMembers)
        {
            var projectHandlerResult = await mediator.Send(new RemoveProjectMemberCommand(projectMember.ProjectId, new(projectMember.MemberId)), cancellationToken);
            if (projectHandlerResult.IsFailed)
            {
                return Result.Fail(projectHandlerResult.Errors);
            }
        }

        var notificationDate = dateTimeProvider.Now();
        foreach(var memberManager in memberManagers)
        {
            jobsService.EnqueueCreateNotification(NotificationFactory.UserLeftOrganization(memberManager.UserId, userName, notificationDate, organization.Id));
        }

        return Result.Ok();
    }
}
