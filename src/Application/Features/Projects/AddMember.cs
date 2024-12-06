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

internal class AddProjectMemberHandler(AppDbContext dbContext, IRepository<Project> projectRepository, IJobsService jobsService) 
    : IRequestHandler<AddProjectMemberCommand, Result>
{
    public async Task<Result> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var isUserAMember = await dbContext.Organizations
            .Include(x => x.Members)
            .AnyAsync(x => x.Id == project.OrganizationId && x.Members.Any(xx => xx.UserId == request.Model.UserId), cancellationToken);
        if (!isUserAMember)
        {
            return Result.Fail(new ApplicationError("User is not a member of the project's organization."));
        }

        var result = project.AddMember(request.Model.UserId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);

        jobsService.EnqueueCreateNotification(NotificationFactory.AddedToProject(request.Model.UserId, project.Id));

        return Result.Ok();
    }
}
