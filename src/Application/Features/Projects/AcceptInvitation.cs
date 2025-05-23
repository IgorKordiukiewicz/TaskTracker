using Application.Common;
using Domain.Projects;

namespace Application.Features.Projects;

public record AcceptProjectInvitationCommand(Guid InvitationId) : IRequest<Result>;

internal class AcceptProjectInvitationCommandValidator : AbstractValidator<AcceptProjectInvitationCommand>
{
    public AcceptProjectInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}

internal class AcceptProjectInvitationHandler(IRepository<Project> projectRepository, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<AcceptProjectInvitationCommand, Result>
{
    public async Task<Result> Handle(AcceptProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetBy(x => x.Invitations.Any(xx => xx.Id == request.InvitationId), cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>($"invitation ID: {request.InvitationId}"));
        }

        var result = project.AcceptInvitation(request.InvitationId, dateTimeProvider.Now());

        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await projectRepository.Update(project, cancellationToken);

        return Result.Ok();
    }
}