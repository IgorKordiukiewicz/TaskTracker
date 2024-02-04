using Application.Common;
using Domain.Organizations;
using Hangfire;

namespace Application.Features.Organizations;

public record RemoveOrganizationMemberCommand(Guid OrganizationId, Guid MemberId) : IRequest<Result>;

internal class RemoveOrganizationMemberCommandValidator : AbstractValidator<RemoveOrganizationMemberCommand>
{
    public RemoveOrganizationMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
    }
}

internal class RemoveOrganizationMemberHandler : IRequestHandler<RemoveOrganizationMemberCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;
    private readonly IBackgroundJobClient _jobClient;
    private readonly IJobsService _jobsService;

    public RemoveOrganizationMemberHandler(IRepository<Organization> organizationRepository, IBackgroundJobClient jobClient, IJobsService jobsService)
    {
        _organizationRepository = organizationRepository;
        _jobClient = jobClient;
        _jobsService = jobsService;
    }

    public async Task<Result> Handle(RemoveOrganizationMemberCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.OrganizationId);
        if (organization is null)
        {
            return Result.Fail(new NotFoundError<Organization>(request.OrganizationId));
        }

        var userId = organization.Members.FirstOrDefault(x => x.Id == request.MemberId)?.UserId;

        var result = organization.RemoveMember(request.MemberId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _organizationRepository.Update(organization);

        _jobClient.Enqueue(() => _jobsService.RemoveUserFromOrganizationProjects(userId!.Value, request.OrganizationId));

        return Result.Ok();
    }
}
