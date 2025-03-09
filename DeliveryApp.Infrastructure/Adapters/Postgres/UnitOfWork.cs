using DeliveryApp.Infrastructure.Adapters.Postgres.Entites;
using MediatR;
using Newtonsoft.Json;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mediator = mediator;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Перекладываем Domain Event в Outbox
        // После выполнения этого метода в DbContext будут находится и сам Aggregate и OutboxMessages
        await SaveDomainEventsInOutboxAsync();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing) _dbContext.Dispose();
            _disposed = true;
        }
    }

    private async Task SaveDomainEventsInOutboxAsync()
    {
        var outboxMessages = _dbContext.ChangeTracker
                                        .Entries<IAggregateRoot>()
                                        .Select(x => x.Entity)
                                        .SelectMany(aggregate =>
                                        {
                                            var domainEvents = aggregate.GetDomainEvents();

                                            aggregate.ClearDomainEvents();
                                            return domainEvents;
                                        })
                                        .Select(domainEvent => new OutboxMessage
                                        {
                                            // Создаем объект OutboxMessage на основ Domain Event
                                            Id = domainEvent.EventId,
                                            OccuredOnUtc = DateTime.UtcNow,
                                            Type = domainEvent.GetType().Name,
                                            Content = JsonConvert.SerializeObject(
                                                domainEvent, 
                                                new JsonSerializerSettings
                                                {
                                                    // Эта настройка нужна, чтобы сериализовать Domain Event с указанием типов
                                                    // Если ее не указать, то десериализатор не поймет в какой тип восстанавливать сообщение
                                                    TypeNameHandling = TypeNameHandling.All
                                                })
                                        })
                                        .ToList();

        // Добавляем OutboxMessages в dbContext
        // После выполнения этой строки в DbContext будут находиться сам Aggregate и OutboxMessages
        await _dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages);
    }

    //public async Task PublishDomainEventsAsync(CancellationToken cancellation = default)
    //{
    //    // Получили агрегаты, в которых есть доменные события
    //    var domainEntities = _dbContext.ChangeTracker
    //        .Entries<IAggregateRoot>()
    //        .Where(x => x.Entity.GetDomainEvents().Any());

    //    // Переложили в отдельную переменную
    //    var domainEvents = domainEntities.SelectMany(x => x.Entity.GetDomainEvents()).ToList();

    //    // Очистили Domain Event в самих агрегатах (поскольку далее они будут отправлены и более не нужны)
    //    domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

    //    // Отправили в MediatR
    //    foreach (var domainEvent in domainEvents)
    //        await _mediator.Publish(domainEvent);
    //}
}