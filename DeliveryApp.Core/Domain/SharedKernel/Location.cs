using CSharpFunctionalExtensions;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public sealed class Location : ValueObject
    {
        public int X { get; }
        public int Y { get; }
        
        /// <summary>
        /// Минимально возможная локация
        /// </summary>
        public static readonly Location MinPoint = new(1,1);

        /// <summary>
        /// Максимально возможная локация
        /// </summary>
        public static readonly Location MaxPoint = new(10, 10);

        [ExcludeFromCodeCoverage]
        private Location(){ }

        private Location (int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Создание экземпляра Location с валидацией
        /// </summary>
        /// <param name="x">X координата</param>
        /// <param name="y">Y координата</param>
        /// <returns></returns>
        public static Result<Location> Create(int x, int y)
        {
            if (x < MinPoint.X || x > MaxPoint.X) return Result.Failure<Location>($"Значение '{nameof(x)}' должно быть в диапазоне от 1 до 10 включительно.");
            if (y < MinPoint.X || y > MaxPoint.Y) return Result.Failure<Location>($"Значение '{nameof(y)}' должно быть в диапазоне от 1 до 10 включительно.");

            return new Location(x, y);
        }

        /// <summary>
        /// Вычисление расстояние между курьером и другой точкой
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CalculateDistance(Location other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        /// <summary>
        /// Создание случайного местоположения для тестирования
        /// </summary>
        /// <returns></returns>
        public static Location CreateRandomPoint()
        {
            return new Location(Random.Shared.Next(MinPoint.X,MaxPoint.X+1), Random.Shared.Next(MinPoint.Y, MaxPoint.Y+1));
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}
