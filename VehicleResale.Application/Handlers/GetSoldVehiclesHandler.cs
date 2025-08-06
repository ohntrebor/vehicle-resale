using AutoMapper;
using MediatR;
using VehicleResale.Application.DTOs;
using VehicleResale.Application.Queries;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class GetSoldVehiclesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<GetSoldVehiclesQuery, IEnumerable<VehicleSaleDto>>
    {
        public async Task<IEnumerable<VehicleSaleDto>> Handle(GetSoldVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await unitOfWork.Vehicles.GetSoldVehiclesAsync();
            var orderedVehicles = vehicles.OrderBy(v => v.Price);
            return mapper.Map<IEnumerable<VehicleSaleDto>>(orderedVehicles);
        }
    }
}