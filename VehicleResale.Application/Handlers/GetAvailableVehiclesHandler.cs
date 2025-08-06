using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VehicleResale.Application.DTOs;
using VehicleResale.Application.Queries;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class GetAvailableVehiclesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<GetAvailableVehiclesQuery, IEnumerable<VehicleDto>>
    {
        public async Task<IEnumerable<VehicleDto>> Handle(GetAvailableVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await unitOfWork.Vehicles.GetAvailableVehiclesAsync();
            var orderedVehicles = vehicles.OrderBy(v => v.Price);
            return mapper.Map<IEnumerable<VehicleDto>>(orderedVehicles);
        }
    }
}