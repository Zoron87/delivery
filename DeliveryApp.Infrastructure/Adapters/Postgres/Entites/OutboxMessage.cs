namespace DeliveryApp.Infrastructure.Adapters.Postgres.Entites;
public sealed class OutboxMessage
{
    /// <summary>
    /// Уникальный идентификатор сообщения
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип сообщения
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Тело сообщения
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime OccuredOnUtc { get; set; }

    /// <summary>
    /// Дата публикации
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }
}
