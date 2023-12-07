using Domain.Users;

namespace Application.Features.Users;

public record RegisterUserCommand(UserRegistrationDto Model) : IRequest<Result>;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Model.AuthenticationId).NotEmpty();
        RuleFor(x => x.Model.Email).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model.LastName).NotEmpty().MaximumLength(100);
    }
}

internal class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
{
    private readonly IRepository<User> _userRepository;

    public RegisterUserHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if(await _userRepository.Exists(x => x.AuthenticationId == request.Model.AuthenticationId))
        {
            return Result.Fail(new ApplicationError("User is already registered in the database."));
        }

        var user = User.Create(request.Model.AuthenticationId, request.Model.Email, request.Model.FirstName, request.Model.LastName);

        await _userRepository.Add(user);

        return Result.Ok();
    }
}
