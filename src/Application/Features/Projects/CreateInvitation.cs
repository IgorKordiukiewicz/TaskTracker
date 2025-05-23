using Application.Common;
using Domain.Projects;
using Domain.Users;

namespace Application.Features.Projects;

public record CreateProjectInvitationCommand(Guid ProjectId, CreateProjectInvitationDto Model) : IRequest<Result>;

internal class CreateProjectInvitationCommandValidator : AbstractValidator<CreateProjectInvitationCommand>
{
    public CreateProjectInvitationCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.UserId).NotEmpty();
        RuleFor(x => x.Model.ExpirationDays).Must(x => x > 0).When(x => x.Model.ExpirationDays is not null);
    }
}

internal class CreateProjectInvitationHandler(AppDbContext dbContext, IRepository<Project> projectRepository, IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateProjectInvitationCommand, Result>
{
    public async Task<Result> Handle(CreateProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Fail(new NotFoundError<Project>(request.ProjectId));
        }

        if (!await dbContext.Users.AnyAsync(x => x.Id == request.Model.UserId, cancellationToken))
        {
            return Result.Fail(new NotFoundError<User>(request.Model.UserId));
        }

        var invitationResult = project.CreateInvitation(request.Model.UserId, dateTimeProvider.Now(), request.Model.ExpirationDays);
        if (invitationResult.IsFailed)
        {
            return Result.Fail(invitationResult.Errors);
        }

        await projectRepository.Update(project, cancellationToken);

        return Result.Ok();
    }
}