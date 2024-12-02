using Domain.Users;

namespace Application.Features.Users;

public record UpdateUserNameCommand(Guid Id, UpdateUserNameDto Model) : IRequest<Result>;

internal class UpdateUserNameCommandValidator : AbstractValidator<UpdateUserNameCommand>
{
    public UpdateUserNameCommandValidator()
    {
        RuleFor(x => x.Model.FirstName).NotEmpty();
        RuleFor(x => x.Model.LastName).NotEmpty();
    }
}

internal class UpdateUserNameHandler(IRepository<User> userRepository) 
    : IRequestHandler<UpdateUserNameCommand, Result>
{
    public async Task<Result> Handle(UpdateUserNameCommand request, CancellationToken cancellationToken)
    {
        var user = (await userRepository.GetById(request.Id, cancellationToken))!;

        user.UpdateName(request.Model.FirstName, request.Model.LastName);
        await userRepository.Update(user, cancellationToken);

        return Result.Ok();
    }
}
