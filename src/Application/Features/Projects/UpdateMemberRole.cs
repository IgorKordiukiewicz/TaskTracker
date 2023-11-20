using Application.Data.Repositories;
using Application.Errors;
using Domain.Projects;

namespace Application.Features.Projects;

public record UpdateProjectMemberRoleCommand(Guid ProjectId, Guid MemberId, UpdateMemberRoleDto Model) : IRequest<Result>;

internal class UpdateProjectMemberRoleCommandValidator : AbstractValidator<UpdateProjectMemberRoleCommand>
{
    public UpdateProjectMemberRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
    }
}

internal class UpdateProjectMemberRoleHandler : IRequestHandler<UpdateProjectMemberRoleCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;

    public UpdateProjectMemberRoleHandler(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(UpdateProjectMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetById(request.ProjectId);
        if(project is null)
        {
            return Result.Fail(new ApplicationError("Project with this ID does not exist."));
        }

        var result = project.RolesManager.UpdateMemberRole(request.MemberId, request.Model.RoleId, project.Members);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _projectRepository.Update(project);
        return Result.Ok();
    }
}
