namespace Application.Errors;

public class NotFoundError<TEntity> : ApplicationError
{
    public NotFoundError(Guid id) 
        : base($"{typeof(TEntity).Name} with ID: {id} does not exist.")
    {
    }

    public NotFoundError(string customReason)
        : base($"{typeof(TEntity).Name} with {customReason} does not exist.")
    {

    }
}
