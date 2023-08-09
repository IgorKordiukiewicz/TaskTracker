using Application.Data.Repositories;
using Domain.Projects;

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
    private readonly IRepository<Project> _projectRepository;

    public RemoveProjectMemberHandler(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId);
        if (project is null)
        {
            return Result.Fail(new Error("Project with this ID does not exist."));
        }

        var result = project.RemoveMember(request.MemberId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _projectRepository.Update(project);

        return Result.Ok();
    }
}
