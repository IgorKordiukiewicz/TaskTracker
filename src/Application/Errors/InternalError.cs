namespace Application.Errors;

public class InternalError : ApplicationError
{
    public InternalError(string message) 
        : base(message)
    {
    }
}
