using Application.Common;
using Domain.Projects;

namespace Application.Features.Projects;

public record CancelProjectInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class CancelProjectInvitationCommandValidator : AbstractValidator<CancelProjectInvitationCommand>
{
    public CancelProjectInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class CancelProjectInvitationHandler(IRepository<Project> projectRepository, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CancelProjectInvitationCommand, Result>
{
    public async Task<Result> Handle(CancelProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>($"invitation ID: {request.InvitationId}"));
        }

        var result = project.CancelInvitation(request.InvitationId, dateTimeProvider.Now());
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);
        return Result.Ok();
    }
}