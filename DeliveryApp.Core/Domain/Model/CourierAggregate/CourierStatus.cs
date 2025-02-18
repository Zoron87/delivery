﻿using CSharpFunctionalExtensions;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Статус курьера
    /// </summary>
    public class CourierStatus : ValueObject
    {
        public static readonly CourierStatus Free = new(nameof(Free).ToLowerInvariant());
        public static readonly CourierStatus Busy = new(nameof(Busy).ToLowerInvariant());

        /// <summary>
        ///     Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private CourierStatus() { }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="name">Название</param>
        private CourierStatus(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }

        public static IEnumerable<CourierStatus> List()
        {
            yield return Free;
            yield return Busy;
        }
    }
}
