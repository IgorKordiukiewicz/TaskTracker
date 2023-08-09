namespace Application.Features.Projects;

public record RemoveProjectMemberCommand(Guid ProjectId, Guid MemberId) : IRequest<Result>;

internal class RemoveProjectMemberCommandValidator : AbstractValidator<RemoveProjectMemberCommand>
{
    public RemoveProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
    }
}

internal class RemoveProjectMemberHandler : IRequestHandler<RemoveProjectMemberCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public RemoveProjectMemberHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == request.ProjectId);
        if (project is null)
        {
            return Result.Fail(new Error("Project with this ID does not exist."));
        }

        var result = project.RemoveMember(request.MemberId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        _dbContext.ProjectMembers.Remove(result.Value);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}
