namespace Application.Errors;

public class NotFoundError<TEntity> : ApplicationError
{
    public NotFoundError(Guid id) 
        : base($"{nameof(TEntity)} with ID: {id} does not exist.")
    {
    }

    public NotFoundError(string customReason)
        : base($"{nameof(TEntity)} with {customReason} does not exist.")
    {

    }
}
