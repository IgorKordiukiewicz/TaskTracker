using Domain.Users;
using Infrastructure.Extensions;

namespace Application.Features.Users;

public record RegisterUserCommand(Guid Id, UserRegistrationDto Model) : IRequest<Result>;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Model.Email).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model.AvatarColor).NotEmpty().Length(7);
    }
}

internal class RegisterUserHandler(IRepository<User> userRepository, AppDbContext dbContext) 
    : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if(await userRepository.Exists(x => x.Id == request.Id, cancellationToken))
        {
            return Result.Fail(new ApplicationError("User is already registered in the database."));
        }

        var user = User.Create(request.Id, request.Model.Email, request.Model.FirstName, request.Model.LastName);

        return await dbContext.ExecuteTransaction(async () =>
        {
            await userRepository.Add(user, cancellationToken);

            dbContext.UsersPresentationData.Add(new()
            {
                UserId = user.Id,
                AvatarColor = request.Model.AvatarColor
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        });
    }
}
