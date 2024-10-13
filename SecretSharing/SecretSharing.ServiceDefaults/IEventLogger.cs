namespace SecretSharing.ServiceDefaults;

public interface IEventLogger
{
    void PostMessage(string eventName, string message);
}