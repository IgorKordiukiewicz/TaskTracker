using Application.Common;
using Domain.Notifications;
using Domain.Projects;

namespace Application.Features.Projects;

public record LeaveProjectCommand(Guid ProjectId, Guid UserId) : IRequest<Result>;

internal class LeaveProjectCommandValidator : AbstractValidator<LeaveProjectCommand>
{
    public LeaveProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class LeaveProjectHandler(IRepository<Project> projectRepository, IJobsService jobsService,
    IDateTimeProvider dateTimeProvider, AppDbContext dbContext)
    : IRequestHandler<LeaveProjectCommand, Result>
{
    public async Task<Result> Handle(LeaveProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.Leave(request.UserId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var userName = await dbContext.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => x.FullName)
            .FirstAsync(cancellationToken);

        await projectRepository.Update(project, cancellationToken);

        var membersToNotify = project.GetMembersWithEditMembersPermission();
        var notificationDate = dateTimeProvider.Now();
        foreach (var memberManager in membersToNotify)
        {
            jobsService.EnqueueCreateNotification(NotificationFactory.UserLeftProject(memberManager.UserId, userName, notificationDate, project.Id));
        }

        return Result.Ok();
    }
}