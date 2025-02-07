using CSharpFunctionalExtensions;
using Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    public class Transport : Entity<int>
    {
        public static readonly Transport Pedestrian = new ( 1, nameof(Pedestrian).ToLowerInvariant(), 1 );
        public static readonly Transport Bicycle = new(2, nameof(Bicycle).ToLowerInvariant(), 2);
        public static readonly Transport Car = new(3, nameof(Car).ToLowerInvariant(), 3);
        
        /// <summary>
        /// Наименование транспорта
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Скорость транспорта
        /// </summary>
        public int Speed { get; }

        [ExcludeFromCodeCoverage]
        private Transport() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Идентификатор транспорта</param>
        /// <param name="name">Наименование транспорта</param>
        /// <param name="speed">Скорость транспорта</param>
        private Transport(int id, string name, int speed) : this()
        { 
            Id = id;
            Name = name;
            Speed = speed;
        }

        /// <summary>
        /// Список всех значений видов транспорта
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Transport> List()
        {
            yield return Pedestrian;
            yield return Bicycle;
            yield return Car;
        }

        /// <summary>
        //// Получить транспорт по названию
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Result<Transport, Error> FromName(string name)
        {
            var transport = List().SingleOrDefault(t => string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return (transport != null) ? transport : Errors.StatusIsWrong();
        }

        /// <summary>
        //// Получить транспорт по идентификатору
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Result<Transport, Error> FromId(int id)
        {
            var transport = List().SingleOrDefault(t => t.Id == id);
            return (transport != null) ? transport : Errors.StatusIsWrong();
        }

        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new Error($"{nameof(Transport).ToLowerInvariant}.is.wrong",
                    $"Неверное значение. Допустимые значения: {nameof(Transport).ToLowerInvariant()}: {string.Join(",", List().Select(x => x.Name))}");
            }
        }
    }
}
