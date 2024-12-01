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

internal class AddProjectMemberHandler : IRequestHandler<AddProjectMemberCommand, Result>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Project> _projectRepository;

    public AddProjectMemberHandler(AppDbContext dbContext, IRepository<Project> projectRepository)
    {
        _dbContext = dbContext;
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var isUserAMember = await _dbContext.Organizations
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

        await _projectRepository.Update(project, cancellationToken);

        return Result.Ok();
    }
}
