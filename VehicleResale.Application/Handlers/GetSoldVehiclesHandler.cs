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
    public class GetSoldVehiclesHandler : IRequestHandler<GetSoldVehiclesQuery, IEnumerable<VehicleSaleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSoldVehiclesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VehicleSaleDto>> Handle(GetSoldVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles = await _unitOfWork.Vehicles.GetSoldVehiclesAsync();
            var orderedVehicles = vehicles.OrderBy(v => v.Price);
            return _mapper.Map<IEnumerable<VehicleSaleDto>>(orderedVehicles);
        }
    }
}