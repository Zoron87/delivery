namespace Primitives;

public interface IUnitOfWork
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task PublishDomainEventsAsync(CancellationToken cancellation = default);
}