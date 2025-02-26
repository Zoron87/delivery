using Dapper;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetCreatedAndAssignedOrders;
public class GetCreatedAndAssignedOrdersHandler : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>
{
    private readonly string _connectionString;

    public GetCreatedAndAssignedOrdersHandler(string connectionString)
    {
        _connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetCreatedAndAssignedOrdersQuery request, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<OrderDTO>(
            @"SELECT id, courier_id, location_x, location_y, status FROM public.orders WHERE status = ANY(@statuses);",
            new { statuses = new[] { OrderStatus.Created.Name, OrderStatus.Assigned.Name } });

        if (result.AsList().Count == 0)
            return null;

        var orders = new List<OrderDTO>();
        foreach (var item in result) orders.Add(MapToOrder(item));

        return new GetCreatedAndAssignedOrdersResponse(result.ToList());
    }

    private OrderDTO MapToOrder(OrderDTO item)
    {
        var location = new LocationDTO { X = item.Location.X, Y = item.Location.Y };

        var order = new OrderDTO
        {
            Id = item.Id,
            Location = location,
        };
        return order;
    }
}
