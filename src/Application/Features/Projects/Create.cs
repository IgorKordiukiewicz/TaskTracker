using Application.Data.Repositories;
using Domain.Projects;
using Microsoft.EntityFrameworkCore;

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

    public CreateProjectHandler(AppDbContext dbContext, IRepository<Project> projectRepository)
    {
        _dbContext = dbContext;
        _projectRepository = projectRepository;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<Guid>(new Error("Organization with this ID does not exist."));
        }

        if(await _projectRepository.Exists(x => x.OrganizationId == request.OrganizationId && x.Name == request.Model.Name))
        {
            return Result.Fail<Guid>(new Error("Project with the same name already exists in this organization."));
        }

        var userId = (await _dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.AuthenticationId == request.UserAuthId)).Id;

        var project = Project.Create(request.Model.Name, request.OrganizationId, userId); // TODO: Should UserId be validated or not since it is coming from internal sources so it should always be valid

        await _projectRepository.Add(project);
        await _dbContext.SaveChangesAsync();

        return project.Id;
    }
}
