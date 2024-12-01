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

internal class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
{
    private readonly IRepository<User> _userRepository;
    private readonly AppDbContext _dbContext;

    public RegisterUserHandler(IRepository<User> userRepository, AppDbContext dbContext)
    {
        _userRepository = userRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if(await _userRepository.Exists(x => x.Id == request.Id, cancellationToken))
        {
            return Result.Fail(new ApplicationError("User is already registered in the database."));
        }

        var user = User.Create(request.Id, request.Model.Email, request.Model.FirstName, request.Model.LastName);

        return await _dbContext.ExecuteTransaction(async () =>
        {
            await _userRepository.Add(user, cancellationToken);

            _dbContext.UsersPresentationData.Add(new()
            {
                UserId = user.Id,
                AvatarColor = request.Model.AvatarColor
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        });
    }
}
