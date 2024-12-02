using Application.Common;
using Domain.Organizations;
using Hangfire;

namespace Application.Features.Organizations;

public record RemoveOrganizationMemberCommand(Guid OrganizationId, RemoveOrganizationMemberDto Model) : IRequest<Result>;

internal class RemoveOrganizationMemberCommandValidator : AbstractValidator<RemoveOrganizationMemberCommand>
{
    public RemoveOrganizationMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Model.MemberId).NotEmpty();
    }
}

internal class RemoveOrganizationMemberHandler(IRepository<Organization> organizationRepository, IBackgroundJobClient jobClient, IJobsService jobsService) 
    : IRequestHandler<RemoveOrganizationMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveOrganizationMemberCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetById(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var userId = organization.Members.FirstOrDefault(x => x.Id == request.Model.MemberId)?.UserId;

        var result = organization.RemoveMember(request.Model.MemberId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await organizationRepository.Update(organization, cancellationToken);

        jobClient.Enqueue(() => jobsService.RemoveUserFromOrganizationProjects(userId!.Value, request.OrganizationId, cancellationToken));

        return Result.Ok();
    }
}
