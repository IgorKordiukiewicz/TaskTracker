﻿using Domain.Organizations;
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

internal class CreateProjectHandler(AppDbContext dbContext, IRepository<Project> projectRepository,
    IRepository<Workflow> workflowRepository, IRepository<TaskRelationshipManager> taskRelationshipManagerRepository) 
    : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Organizations.AnyAsync(x => x.Id == request.Model.OrganizationId, cancellationToken))
        {
            return Result.Fail<Guid>(new NotFoundError<Organization>(request.Model.OrganizationId));
        }

        if(await projectRepository.Exists(x => x.OrganizationId == request.Model.OrganizationId && x.Name == request.Model.Name, cancellationToken))
        {
            return Result.Fail<Guid>(new ApplicationError("Project with the same name already exists in this organization."));
        }

        var project = Project.Create(request.Model.Name, request.Model.OrganizationId, request.UserId);
        var workflow = Workflow.Create(project.Id);
        var taskRelationshipManager = new TaskRelationshipManager(project.Id);

        var result = await dbContext.ExecuteTransaction(async () =>
        {           
            await projectRepository.Add(project, cancellationToken);
            await workflowRepository.Add(workflow, cancellationToken);
            await taskRelationshipManagerRepository.Add(taskRelationshipManager, cancellationToken);
        });

        if(result.IsFailed)
        {
            return Result.Fail<Guid>(result.Errors);
        }

        return project.Id;
    }
}
