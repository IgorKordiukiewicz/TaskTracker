namespace Application.Features.Projects;

public record AddProjectMemberCommand(Guid ProjectId, AddProjectMemberDto Model) : IRequest<Result>;

internal class AddProjectMemberHandler : IRequestHandler<AddProjectMemberCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public AddProjectMemberHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.ProjectId);
        if(project is null)
        {
            return Result.Fail(new Error("Project with this ID does not exist."));
        }

        var organization = await _dbContext.Organizations
            .Include(x => x.Members)
            .FirstAsync(x => x.Id == project.OrganizationId);

        var isUserAMember = organization.IsUserAMember(request.Model.UserId);
        if(!isUserAMember)
        {
            return Result.Fail(new Error("User is not a member of the project's organization."));
        }

        var result = project.AddMember(request.Model.UserId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _dbContext.ProjectMembers.AddAsync(result.Value);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}
