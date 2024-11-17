using Domain.Organizations;
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
        RuleFor(x => x.Model.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
    }
}

internal class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<Workflow> _workflowRepository;
    private readonly IRepository<TaskRelationshipManager> _taskRelationshipManagerRepository;

    public CreateProjectHandler(AppDbContext dbContext, IRepository<Project> projectRepository, 
        IRepository<Workflow> workflowRepository, IRepository<TaskRelationshipManager> taskRelationshipManagerRepository)
    {
        _dbContext = dbContext;
        _projectRepository = projectRepository;
        _workflowRepository = workflowRepository;
        _taskRelationshipManagerRepository = taskRelationshipManagerRepository;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.Model.OrganizationId))
        {
            return Result.Fail<Guid>(new NotFoundError<Organization>(request.Model.OrganizationId));
        }

        if(await _projectRepository.Exists(x => x.OrganizationId == request.Model.OrganizationId && x.Name == request.Model.Name))
        {
            return Result.Fail<Guid>(new ApplicationError("Project with the same name already exists in this organization."));
        }

        var project = Project.Create(request.Model.Name, request.Model.OrganizationId, request.UserId);
        var workflow = Workflow.Create(project.Id);
        var taskRelationshipManager = new TaskRelationshipManager(project.Id);

        var result = await _dbContext.ExecuteTransaction(async () =>
        {           
            await _projectRepository.Add(project);
            await _workflowRepository.Add(workflow);
            await _taskRelationshipManagerRepository.Add(taskRelationshipManager);
        });

        if(result.IsFailed)
        {
            return Result.Fail<Guid>(result.Errors);
        }

        return project.Id;
    }
}
