using Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projects;

public record CreateProjectCommand(Guid OrganizationId, CreateProjectDto Model) : IRequest<Result<Guid>>;

internal class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly AppDbContext _dbContext;

    public CreateProjectHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var organization = await _dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId);
        if(organization is null)
        {
            return Result.Fail<Guid>(new Error("Organization with this ID does not exist."));
        }

        if(await _dbContext.Projects.AnyAsync(x => x.OrganizationId == request.OrganizationId && x.Name == request.Model.Name))
        {
            return Result.Fail<Guid>(new Error("Project with the same name already exists in this organization."));
        }

        var project = Project.Create(request.Model.Name, request.OrganizationId);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        return project.Id;
    }
}
