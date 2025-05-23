using Application.Common;
using Domain.Notifications;
using Domain.Projects;

namespace Application.Features.Projects;

public record AddProjectMemberCommand(Guid ProjectId, AddProjectMemberDto Model) : IRequest<Result>;

internal class AddProjectMemberCommandValidator : AbstractValidator<AddProjectMemberCommand>
{
    public AddProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.UserId).NotEmpty();
    }
}

internal class AddProjectMemberHandler(AppDbContext dbContext, IRepository<Project> projectRepository, 
    IJobsService jobsService, IDateTimeProvider dateTimeProvider) 
    : IRequestHandler<AddProjectMemberCommand, Result>
{
    public async Task<Result> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        // TODO

        var result = project.AddMember(request.Model.UserId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);

        jobsService.EnqueueCreateNotification(NotificationFactory.AddedToProject(request.Model.UserId, dateTimeProvider.Now(), project.Id));

        return Result.Ok();
    }
}
