namespace SmartShop.Loyalty.Infra.Data.Inbox;

public sealed class ProcessedMessage
{
    private ProcessedMessage()
    {
        Type = string.Empty;
    }

    public ProcessedMessage(Guid id, string type)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Message id is required.", nameof(id));
        }

        Id = id;
        Type = type;
        ProcessedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public DateTime ProcessedAtUtc { get; private set; }
}
