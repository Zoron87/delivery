using Api.Controllers;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Queries.GetCouriers;
using DeliveryApp.Core.Application.Queries.GetCreatedAndAssignedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Api.Adapters.Http
{
    public class DeliveryController : DefaultApiController
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            var street = "Несуществующая";
            var createOrderCommand = new CreateOrderCommand(orderId, street);
            var response = await _mediator.Send(createOrderCommand);
            if (response.IsSuccess) return Ok();
            return Conflict();
        }

        public override async Task<IActionResult> GetCouriers()
        {
            var getCouriersQuery = new GetCouriersQuery();
            var result = await _mediator.Send(getCouriersQuery);

            if (result == null) return NotFound();
            var courier = result.Couriers.Select(c => new CourierDTO
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                TransportId = c.TransportId
            });
            return Ok(courier);
        }

        public override async Task<IActionResult> GetOrders()
        {
            var GetCreatedAndAssignedOrdersQuery = new GetCreatedAndAssignedOrdersQuery();
            var result = await _mediator.Send(GetCreatedAndAssignedOrdersQuery);

            if (result == null) NotFound();
            var order = result.Orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                Location = o.Location
            });
            return Ok(order);

        }
    }
}
