using Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projects;

public record CreateProjectCommand(CreateProjectDto Model) : IRequest<Result<Guid>>;

internal class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Model.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model.OrganizationId).NotEmpty();
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
        var organization = await _dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.Model.OrganizationId);
        if(organization is null)
        {
            return Result.Fail<Guid>(new Error("Organization with this ID does not exist."));
        }

        // TODO: Disallow creating projects with same name in organization

        var project = Project.Create(request.Model.Name, request.Model.OrganizationId);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        return project.Id;
    }
}
