namespace PhlegmaticOne.UnitOfWork.Implementation.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? ProceedAtUtc { get; set; }
    public string Error { get; set; } = string.Empty;
}