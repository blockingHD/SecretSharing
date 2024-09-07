namespace SecretSharing.ServiceDefaults.Attribute;

public record Event(string EventName)
{
    public static implicit operator string(Event @event)
    {
        return @event.EventName;
    }
}