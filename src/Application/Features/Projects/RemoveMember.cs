using Application.Common;
using Domain.Notifications;
using Domain.Projects;
using Infrastructure.Extensions;

namespace Application.Features.Projects;

public record RemoveProjectMemberCommand(Guid ProjectId, RemoveProjectMemberDto Model) : IRequest<Result>;

internal class RemoveProjectMemberCommandValidator : AbstractValidator<RemoveProjectMemberCommand>
{
    public RemoveProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.MemberId).NotEmpty();
    }
}

internal class RemoveProjectMemberHandler(IRepository<Project> projectRepository, AppDbContext dbContext, 
    IJobsService jobsService, IDateTimeProvider dateTimeProvider) 
    : IRequestHandler<RemoveProjectMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var userId = project.Members.FirstOrDefault(x => x.Id == request.Model.MemberId)?.UserId;

        var result = project.RemoveMember(request.Model.MemberId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var transactionResult = await dbContext.ExecuteTransaction(async () =>
        {
            await projectRepository.Update(project, cancellationToken);

            await dbContext.Tasks
                .Where(x => x.ProjectId == project.Id && x.AssigneeId == userId!.Value)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.AssigneeId, (Guid?)null), cancellationToken);
        });

        jobsService.EnqueueCreateNotification(NotificationFactory.RemovedFromProject(userId!.Value, dateTimeProvider.Now(), project.Id));

        return transactionResult;
    }
}
