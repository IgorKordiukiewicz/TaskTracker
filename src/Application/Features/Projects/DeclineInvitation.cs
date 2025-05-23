using Application.Common;
using Domain.Projects;

namespace Application.Features.Projects;

public record DeclineProjectInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class DeclineProjectInvitationCommandValidator : AbstractValidator<DeclineProjectInvitationCommand>
{
    public DeclineProjectInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class DeclineProjectInvitationHandler(IRepository<Project> projectRepository, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<DeclineProjectInvitationCommand, Result>
{
    public async Task<Result> Handle(DeclineProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>($"invitation ID: {request.InvitationId}"));
        }

        var result = project.DeclineInvitation(request.InvitationId, dateTimeProvider.Now());

        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);

        return Result.Ok();
    }
}