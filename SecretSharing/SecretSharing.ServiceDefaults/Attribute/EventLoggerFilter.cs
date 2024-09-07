using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SecretSharing.ServiceDefaults.Attribute;

public class EventLoggerFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        var result = await next(context);

        var eventName = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<Event>();

        if (eventName is not null)
        {
            var eventLogger = context.HttpContext.RequestServices.GetRequiredService<IEventLogger>();

            eventLogger.PostMessage(eventName,
                EventLoggerBody.CreateFromHttpContext(context.HttpContext));
        }

        return result;
    }
}