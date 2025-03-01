using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.AssignCourier;
public sealed class AssignCourierCommand : IRequest<UnitResult<Error>>
{
}
