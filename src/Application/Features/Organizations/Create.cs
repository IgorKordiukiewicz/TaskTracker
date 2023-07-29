using Application.Data;
using Domain.Organizations;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;

namespace Application.Features.Organizations;

public record CreateOrganizationCommand(CreateOrganizationDto Model) : IRequest<Result<Guid>>;

internal class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Model.OwnerId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
    }
}

internal class CreateOrganizationHandler : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    private readonly AppDbContext _context;

    public CreateOrganizationHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if(!await _context.Users.AnyAsync(x => x.Id == request.Model.OwnerId))
        {
            return Result.Fail(new Error("Owner does not exist"));
        }

        var organization = Organization.Create(request.Model.Name, request.Model.OwnerId);

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        return organization.Id;
    }
}
