using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleResale.Application.Commands;
using VehicleResale.Application.DTOs;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, VehicleDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateVehicleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<VehicleDto> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.Id);
            
            if (vehicle == null)
                throw new InvalidOperationException($"Vehicle with ID {request.Id} not found");

            vehicle.UpdateDetails(
                request.Brand,
                request.Model,
                request.Year,
                request.Color,
                request.Price
            );

            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleDto>(vehicle);
        }
    }
}