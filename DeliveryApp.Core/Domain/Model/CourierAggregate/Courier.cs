using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Курьер
    /// </summary>
    public class Courier : Aggregate<Guid>
    {
        /// <summary>
        /// Имя курьера
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Транспорт курьера
        /// </summary>
        public Transport Transport { get; private set; }

        /// <summary>
        /// Текущее местоположение курьера
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Статус курьера
        /// </summary>
        public CourierStatus Status { get; private set; }

        [ExcludeFromCodeCoverage]
        private Courier() {}

        /// <summary>
        /// Конструктор для создания курьера
        /// </summary>
        /// <param name="name">Имя курьера</param>
        /// <param name="transport">Транспорт курьера</param>
        /// <param name="location"></param>
        private Courier(string name, Transport transport, Location location) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Transport = transport;
            Location = location;
            Status = CourierStatus.Free;
        }

        /// <summary>
        /// Фабричный метод для создания курьера
        /// </summary>
        /// <param name="name">Имя курьера</param>
        /// <param name="transport">Транспорт курьера</param>
        /// <param name="location">Адрес курьера</param>
        /// <returns></returns>
        public static Result<Courier, Error> Create(string name, Transport transport, Location location)
        {
            if (name == null) return GeneralErrors.ValueIsRequired(nameof(name).ToLowerInvariant());
            if (transport == null) return GeneralErrors.ValueIsRequired(nameof(transport).ToLowerInvariant());
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location).ToLowerInvariant());

            return new Courier(name, transport, location);
        }

        /// <summary>
        /// Сделать курьера свободным
        /// </summary>
        /// <returns></returns>
        public UnitResult<Error> SetFree()
        {
            this.Status = CourierStatus.Free;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Сделать курьера занятым
        /// </summary>
        /// <returns></returns>
        public UnitResult<Error> SetBusy()
        {
            if (Status == CourierStatus.Busy) return Errors.TryAssignOrderWhenCourierBusy();

            this.Status = CourierStatus.Busy;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Получить кол-во шагов до адреса заказа
        /// </summary>
        /// <param name="location">Адрес заказа</param>
        /// <returns></returns>
        public Result<double, Error> GetNumberOfSteps(Location location)
        {
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

            var distance = this.Location.CalculateDistance(location);
            return distance / this.Transport.Speed;
        }

        /// <summary>
        /// Сделать один шаг в направлении заказа
        /// </summary>
        /// <param name="finalLocation">Адрес заказа</param>
        /// <returns></returns>
        public Result<Location, Error> MoveStep(Location finalLocation)
        {
            if (finalLocation == null) return GeneralErrors.ValueIsRequired(nameof(finalLocation).ToLowerInvariant());

            var oneStep = this.Transport.Speed;

            while (oneStep > 0 && this.Location.X != finalLocation.X)
            {
                if (this.Location.X < finalLocation.X)
                    this.Location = Location.Create(this.Location.X + 1, this.Location.Y).Value;
                
                else if (this.Location.X > finalLocation.X)
                    this.Location = Location.Create(this.Location.X - 1, this.Location.Y).Value;
               
                oneStep--;
            }

            while (oneStep > 0 && this.Location.Y != finalLocation.Y)
            {
                if (this.Location.Y < finalLocation.Y)
                    this.Location = Location.Create(this.Location.X, this.Location.Y + 1).Value;
                else if (this.Location.Y > finalLocation.Y)
                    this.Location = Location.Create(this.Location.X, this.Location.Y - 1).Value;

                oneStep--;
            }

            return this.Location;
        }

        /// <summary>
        /// Возможные ошибки сущности курьера
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static class Errors
        {
            public static Error TryAssignOrderWhenCourierBusy()
            {
                return new Error($"{nameof(Courier).ToLowerInvariant} try assign order when courier busy.",
                    "Нельзя назначить заказ, если курьер уже занят");
            }
        }
    }
}
