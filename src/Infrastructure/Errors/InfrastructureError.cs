using FluentResults;

namespace Infrastructure.Errors;

public class InfrastructureError : Error
{
    public InfrastructureError(string message)
        : base(message) { }
}
