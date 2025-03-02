namespace DeliveryApp.Core.Application.Queries.GetCouriers;

public class GetCourierResponse
{
    public GetCourierResponse(List<CourierDTO> couriers)
    {
        Couriers.AddRange(couriers);
    }

    public List<CourierDTO> Couriers { get; set; } = new();
}

public class CourierDTO
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Вид транспорта
    /// </summary>
    public int TransportId { get; set; }

    /// <summary>
    ///  Геопозиция (X,Y)
    /// </summary>
    public LocationDTO Location { get; set; } = new();
}

public class LocationDTO
{
    /// <summary>
    /// Горизонталь
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///  Вертикаль
    /// </summary>
    public int Y { get; set; }
}
