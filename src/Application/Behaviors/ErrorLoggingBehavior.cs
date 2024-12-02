using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public class ErrorLoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (response is null)
        {
            return response;
        }

        var errors = typeof(TResponse).GetProperty("Errors")?.GetValue(response) as List<IError>;
        var error = errors?.FirstOrDefault();
        if (error is not null)
        {
            logger.LogError("Request: \"{@request}\" failed with error message: \"{@errorMessage}\"", request.ToString(), error.Message);
        }

        return response;
    }
}
