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

        var result = await connection.QueryAsync<OrderDTO, LocationDTO, OrderDTO>
         (
            @"SELECT id, courier_id, status, location_x AS ""X"", location_y as ""Y"" FROM public.orders WHERE status = ANY(@statuses);",

            (order, location) =>
            {
                order.Location = location;
                return order;
            },
            splitOn: "X",
            param: new { statuses = new[] { OrderStatus.Created.Name, OrderStatus.Assigned.Name } }
         );

        if (result.AsList().Count == 0)
            return null;

        return new GetCreatedAndAssignedOrdersResponse(result.ToList());
    }
}
