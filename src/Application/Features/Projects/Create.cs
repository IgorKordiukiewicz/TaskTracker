﻿using Domain.Organizations;
using Domain.Projects;
using Domain.Tasks;
using Domain.Workflows;

namespace Application.Features.Projects;

public record CreateProjectCommand(string UserAuthId, CreateProjectDto Model) : IRequest<Result<Guid>>;

internal class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.UserAuthId).NotEmpty();
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

        var userId = (await _dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.AuthenticationId == request.UserAuthId)).Id;

        // TODO: Transaction?

        var project = Project.Create(request.Model.Name, request.Model.OrganizationId, userId);
        await _projectRepository.Add(project);

        var workflow = Workflow.Create(project.Id);
        await _workflowRepository.Add(workflow);

        var taskRelationshipManager = new TaskRelationshipManager(project.Id);
        await _taskRelationshipManagerRepository.Add(taskRelationshipManager);

        return project.Id;
    }
}
