using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetCouriers;
public class GetCouriersQueryHandler : IRequestHandler<GetCouriersQuery, GetCourierResponse>
{
    private readonly string _connectionString;
    public GetCouriersQueryHandler(string connectionString)
    {
        _connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<GetCourierResponse> Handle(GetCouriersQuery request, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<CourierDTO, LocationDTO, CourierDTO>(
            @"SELECT id, name, transport_id as ""TransportId"", location_x AS ""X"", location_y AS ""Y"" 
              FROM public.couriers"
            , 
            (courier, location) =>
            {
                courier.Location = location;
                return courier;
            },
        splitOn: "X");

        if (result.AsList().Count == 0)
            return null;

        return new GetCourierResponse(result.ToList());
    }
}
