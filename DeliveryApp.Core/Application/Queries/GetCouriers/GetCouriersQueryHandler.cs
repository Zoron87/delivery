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

        var result = await connection.QueryAsync<CourierDTO>(
            @"SELECT id, name, location_x, location_y, status, transport_id FROM public.couriers"
            , cancellationToken);

        if (result.AsList().Count == 0)
            return null;

        var couriers = new List<CourierDTO>();

        couriers = result.Select(item => MapToCourier(item)).ToList();

        return new GetCourierResponse(couriers);
    }

    private CourierDTO MapToCourier(CourierDTO item)
    {
        var location = new LocationDTO { X = item.Location.X, Y = item.Location.Y };

        var courier = new CourierDTO
        {
            Id = item.Id,
            Name = item.Name,
            Location = location,
            TransportId = item.TransportId,
        };
        return courier;
    }

}
