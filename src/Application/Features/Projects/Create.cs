using Application.Common;
using Domain.Projects;
using Domain.Tasks;
using Domain.Workflows;
using Infrastructure.Extensions;

namespace Application.Features.Projects;

public record CreateProjectCommand(Guid UserId, CreateProjectDto Model) : IRequest<Result<Guid>>;

internal class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
    }
}

internal class CreateProjectHandler(AppDbContext dbContext, IRepository<Project> projectRepository, IRepository<Workflow> workflowRepository, 
    IRepository<TaskRelationManager> taskRelationManagerRepository, ITasksBoardLayoutService tasksBoardLayoutService) 
    : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // TODO: Make name unique or not?
        //if(await projectRepository.Exists(x => x.Name == request.Model.Name, cancellationToken))
        //{
        //    return Result.Fail<Guid>(new ApplicationError("Project with the same name already exists."));
        //}

        var project = Project.Create(request.Model.Name, request.UserId);
        var workflow = Workflow.Create(project.Id);
        var relationManager = new TaskRelationManager(project.Id);

        var result = await dbContext.ExecuteTransaction(async () =>
        {           
            await projectRepository.Add(project, cancellationToken);
            await workflowRepository.Add(workflow, cancellationToken);
            await taskRelationManagerRepository.Add(relationManager, cancellationToken);
            await tasksBoardLayoutService.HandleChanges(project.Id, 
                layout => layout.Initialize(workflow.Statuses.Select(x => x.Id)), cancellationToken);
        });

        if(result.IsFailed)
        {
            return Result.Fail<Guid>(result.Errors);
        }

        return project.Id;
    }
}
