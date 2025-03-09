using DeliveryApp.Infrastructure.Adapters.Postgres.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Primitives;
using Quartz;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.BackgroundJobs;
public class ProcessOutboxMessagesJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Получаем все DomainEvents, которые еще не были отправлены (где ProcessedOnUtc == null)
        var outboxMessages = await _dbContext
                                        .Set<OutboxMessage>()
                                        .Where(m => m.ProcessedOnUtc == null)
                                        .OrderBy(o => o.OccuredOnUtc)
                                        .Take(20)
                                        .ToListAsync(context.CancellationToken);

        // Если такие есть, то перебираем их в цикле
        if (outboxMessages.Any())
        {
            foreach (var outboxMessage in outboxMessages)
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content, settings);

                await _mediator.Publish(domainEvent, context.CancellationToken);

                // Если предыдущий метод не вернул ошибку, значит отправка была успешной
                // Ставим дату отправки, это будет признаком, что сообщениеотправлять больше не нужно
                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}