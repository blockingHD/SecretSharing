namespace Microsoft.Extensions.Hosting;

public interface IEventLogger
{
    void PostMessage(string eventName, string message);
}