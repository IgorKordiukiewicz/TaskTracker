namespace Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = validators.Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var res = new TResponse();
            res.Reasons.Add(new ValidationError(failures));
            return res;
        }

        return await next();
    }
}
