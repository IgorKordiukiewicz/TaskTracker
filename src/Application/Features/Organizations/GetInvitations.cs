using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationInvitationsQuery(Guid OrganizationId, Pagination Pagination) : IRequest<Result<OrganizationInvitationsVM>>;

internal class GetOrganizationInvitationsQueryValidator : AbstractValidator<GetOrganizationInvitationsQuery>
{
    public GetOrganizationInvitationsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Pagination).SetValidator(new PaginationValidator());
    }
}

internal class GetOrganizationInvitationsHandler : IRequestHandler<GetOrganizationInvitationsQuery, Result<OrganizationInvitationsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationInvitationsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationInvitationsVM>> Handle(GetOrganizationInvitationsQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<OrganizationInvitationsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var query = _dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(x => x.OrganizationId == request.OrganizationId)
            .Join(_dbContext.Users,
            invitation => invitation.UserId,
            user => user.Id,
            (invitation, user) => new { Invitation = invitation, User = user });

        var totalPagesCount = request.Pagination.GetPagesCount(await query.CountAsync());

        var invitations = await query
            .OrderByDescending(x => x.Invitation.CreatedAt)
            .Skip(request.Pagination.Offset)
            .Take(request.Pagination.ItemsPerPage)
            .Select(x => new OrganizationInvitationVM(x.Invitation.Id, x.User.Email, x.Invitation.State, x.Invitation.CreatedAt, x.Invitation.FinalizedAt))
            .ToListAsync();

        return Result.Ok(new OrganizationInvitationsVM(invitations, totalPagesCount));
    }
}
