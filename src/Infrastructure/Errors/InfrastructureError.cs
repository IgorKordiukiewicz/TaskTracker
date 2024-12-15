using FluentResults;

namespace Infrastructure.Errors;

public class InfrastructureError(string message) 
    : Error(message)
{
}
