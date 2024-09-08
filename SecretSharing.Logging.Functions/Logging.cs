using Microsoft.Azure.Functions.Worker;
using SecretSharing.ServiceDefaults.Attribute;

namespace SecretSharing.Logging.Functions;

public static class Logging
{
    [Function("Logging")]
    public static void Run(
        [RabbitMQTrigger("logging", ConnectionStringSetting = "messaging")]
        EventLoggerBody subEvent,
        FunctionContext functionContext)
    {
    }
}