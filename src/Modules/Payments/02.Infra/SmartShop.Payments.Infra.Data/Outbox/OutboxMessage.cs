namespace SmartShop.Payments.Infra.Data.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
        Type = string.Empty;
        RoutingKey = string.Empty;
        Payload = string.Empty;
    }

    public OutboxMessage(
        Guid id,
        string type,
        string routingKey,
        string payload,
        DateTime occurredAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Message id is required.", nameof(id));
        }

        Id = id;
        Type = type;
        RoutingKey = routingKey;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public string RoutingKey { get; private set; }

    public string Payload { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ProcessedAtUtc { get; private set; }

    public int Attempts { get; private set; }

    public string? LastError { get; private set; }

    public void MarkProcessed()
    {
        ProcessedAtUtc = DateTime.UtcNow;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        Attempts++;
        LastError = error.Length <= 2000 ? error : error[..2000];
    }
}
