using DeliveryApp.Core.Application.Queries.GetCouriers;

namespace DeliveryApp.Core.Application.Queries.GetCreatedAndAssignedOrders;
public class GetCreatedAndAssignedOrdersResponse
{
    public GetCreatedAndAssignedOrdersResponse(List<OrderDTO> orders)
    {
        Orders.AddRange(orders);
    }

    public List<OrderDTO> Orders { get; set; } = new();
}

public class OrderDTO
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Геопозиция (X,Y)
    /// </summary>
    public LocationDTO Location { get; set; }
}

public class LocationDTO
{
    /// <summary>
    /// Горизонталь
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Вертикаль
    /// </summary>
    public int Y { get; set; }
}
