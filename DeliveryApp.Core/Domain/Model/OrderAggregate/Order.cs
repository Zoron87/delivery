using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order : Aggregate<Guid>
    {
        /// <summary>
        /// Адрес заказа
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// Идентификатор курьера
        /// </summary>
        public Guid? CourierId { get; private set; }
        
        [ExcludeFromCodeCoverage]
        private Order() { }
       
        /// <summary>
        /// Конструктор заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="location">Адрес заказа</param>
        private Order (Guid id, Location location) : this()
        {
            Id = id;
            Location = location;
            Status = OrderStatus.Created;
        }

        /// <summary>
        /// Создать заказ
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="location">Адрес заказа</param>
        /// <returns></returns>
        public static Result<Order, Error> Create(Guid id, Location location)
        {
            if (id == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(id));
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

            return new Order(id, location);
        }
       
        /// <summary>
        /// Назначить заказ на курьера
        /// </summary>
        /// <param name="courierId"></param>
        /// <returns></returns>
        public UnitResult<Error> AssignCourier(Courier courier)
        {
            if (courier.Id == Guid.Empty) GeneralErrors.ValueIsRequired(nameof(courier));

            this.Status = OrderStatus.Assigned;
            this.CourierId = courier.Id;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Завершить заказ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UnitResult<Error> Completed()
        {
            if (Status != OrderStatus.Assigned) return Errors.CantCompleteNotAssignedOrder();

            if (Status == OrderStatus.Assigned)
                Status = OrderStatus.Completed;
            else Errors.CantCompleteNotAssignOrder();

            RaiseDomainEvent(new OrderStatusChangedDomainEvent(Id, Status.Name));

            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Возможные ошибки сущности заказа
        /// </summary>
        private static class Errors
        {
            public static Error CantCompleteNotAssignOrder()
            {
                return new Error($"{nameof(Order).ToLowerInvariant}.cant completed not assign order",
                    "Нельзя завершить заказ, который не был назначен!");
            }

            public static Error CantCompleteNotAssignedOrder()
            {
                return new Error($"{nameof(Order).ToLowerInvariant}cant completed not assigned order.",
                    "Нельзя завершить заказ, который не был никому назначен.");
            }
        }
    }
}
