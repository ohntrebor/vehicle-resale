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
    public class GetAvailableVehiclesHandler : IRequestHandler<GetAvailableVehiclesQuery, IEnumerable<VehicleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAvailableVehiclesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VehicleDto>> Handle(GetAvailableVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _unitOfWork.Vehicles.GetAvailableVehiclesAsync();
            var orderedVehicles = vehicles.OrderBy(v => v.Price);
            return _mapper.Map<IEnumerable<VehicleDto>>(orderedVehicles);
        }
    }
}