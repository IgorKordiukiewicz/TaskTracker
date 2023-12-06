using Application.Data.Repositories;
using Application.Errors;
using Domain.Organizations;
using Domain.Projects;
using Domain.Workflows;

namespace Application.Features.Projects;

public record CreateProjectCommand(Guid OrganizationId, string UserAuthId, CreateProjectDto Model) : IRequest<Result<Guid>>;

internal class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.UserAuthId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
    }
}

internal class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<Workflow> _workflowRepository;

    public CreateProjectHandler(AppDbContext dbContext, IRepository<Project> projectRepository, IRepository<Workflow> workflowRepository)
    {
        _dbContext = dbContext;
        _projectRepository = projectRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<Guid>(new NotFoundError<Organization>(request.OrganizationId));
        }

        if(await _projectRepository.Exists(x => x.OrganizationId == request.OrganizationId && x.Name == request.Model.Name))
        {
            return Result.Fail<Guid>(new ApplicationError("Project with the same name already exists in this organization."));
        }

        var userId = (await _dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.AuthenticationId == request.UserAuthId)).Id;

        var project = Project.Create(request.Model.Name, request.OrganizationId, userId);
        await _projectRepository.Add(project);

        var workflow = Workflow.Create(project.Id);
        await _workflowRepository.Add(workflow);

        return project.Id;
    }
}
