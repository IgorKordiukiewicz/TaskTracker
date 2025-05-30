﻿using Domain.Projects;

namespace Application.Features.Projects;

public record DeleteProjectRoleCommand(Guid ProjectId, DeleteRoleDto Model) : IRequest<Result>;

internal class DeleteProjectRoleCommandValidator : AbstractValidator<DeleteProjectRoleCommand>
{
    public DeleteProjectRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.RoleId).NotEmpty();
    }
}

internal class DeleteProjectRoleHandler(IRepository<Project> projectRepository) 
    : IRequestHandler<DeleteProjectRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectRoleCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if(project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        var result = project.RolesManager.DeleteRole(request.Model.RoleId, project.Members);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);
        return Result.Ok();
    }
}
