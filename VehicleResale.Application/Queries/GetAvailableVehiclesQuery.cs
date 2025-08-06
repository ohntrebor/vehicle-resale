using MediatR;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Queries
{
    public class GetAvailableVehiclesQuery : IRequest<IEnumerable<VehicleDto>>
    {
    }
}