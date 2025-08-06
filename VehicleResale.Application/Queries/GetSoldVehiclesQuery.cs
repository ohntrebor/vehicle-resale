using MediatR;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Queries
{
    public class GetSoldVehiclesQuery : IRequest<IEnumerable<VehicleSaleDto>>
    {
    }
}