using DeliveryApp.Core.Domain.Model.SharedKernel;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    /// <summary>
    ///     Получить информацию о геолокации по улице
    /// </summary>
    Task<Location> GetGeolocationAsync(string street, CancellationToken cancellationToken = default);
}