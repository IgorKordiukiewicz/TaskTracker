using Domain.Projects;

namespace Application.Features.Projects;

public record RemoveProjectMemberCommand(Guid ProjectId, RemoveProjectMemberDto Model) : IRequest<Result>;

internal class RemoveProjectMemberCommandValidator : AbstractValidator<RemoveProjectMemberCommand>
{
    public RemoveProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.MemberId).NotEmpty();
    }
}

internal class RemoveProjectMemberHandler : IRequestHandler<RemoveProjectMemberCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;
    private readonly AppDbContext _dbContext;

    public RemoveProjectMemberHandler(IRepository<Project> projectRepository, AppDbContext dbContext)
    {
        _projectRepository = projectRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var userId = project.Members.FirstOrDefault(x => x.Id == request.Model.MemberId)?.UserId;

        var result = project.RemoveMember(request.Model.MemberId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await _projectRepository.Update(project);

            await _dbContext.Tasks
                .Where(x => x.ProjectId == project.Id && x.AssigneeId == userId!.Value)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.AssigneeId, (Guid?)null));

            await transaction.CommitAsync();
        }
        catch(Exception ex)
        {
            return Result.Fail(new InternalError("SQL Transaction failure").CausedBy(ex));
        }

        return Result.Ok();
    }
}
