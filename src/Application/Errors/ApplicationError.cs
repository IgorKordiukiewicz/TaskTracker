namespace Application.Errors;

public class ApplicationError : Error
{
    public ApplicationError(string message)
        : base(message) { }
}
