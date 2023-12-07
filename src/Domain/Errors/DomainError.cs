namespace Domain.Errors;

public class DomainError : Error
{
    public DomainError(string message)
        : base(message) { }
}
