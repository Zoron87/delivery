using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        /// <summary>
        /// Назначить заказ на курьера
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="couriers">Список курьеров</param>
        /// <returns></returns>
        public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
        {
            if (couriers == null || couriers.Count == 0) return GeneralErrors.InvalidLength(nameof(couriers));
            if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));
            if (order.Status == OrderStatus.Completed) return Errors.CurrentOrderCompleted();
            if (order.Status == OrderStatus.Assigned) return Errors.CurrentOrderAssigned();

            var suitableCourier = couriers.Where(c => c.Status == CourierStatus.Free)
                                          .MinBy(c => c.GetNumberOfSteps(order.Location).Value);

            if (suitableCourier == null) return Errors.SuitableCourierNotFound();

            var tryAssignOrderToCourier = order.AssignCourier(suitableCourier);
            if (tryAssignOrderToCourier.IsFailure)  return tryAssignOrderToCourier.Error;

            var tryCourierSetBusy = suitableCourier.SetBusy();
            if (tryCourierSetBusy.IsFailure) return tryCourierSetBusy.Error;

            return suitableCourier;
        }

        /// <summary>
        /// Ошибки, которые может возвращать класс
        /// </summary>
        public static class Errors
        {
            public static Error SuitableCourierNotFound() =>
                new Error("suitable courier was not found", "Подходящий курьер не был найден!");

            public static Error CurrentOrderCompleted() =>
                new Error("current order was completed", "Данный заказ уже завершен!");

            public static Error CurrentOrderAssigned() =>
                new Error("current order was assigned", "Данный заказ уже назначен курьеру!");
        }
    }
}
