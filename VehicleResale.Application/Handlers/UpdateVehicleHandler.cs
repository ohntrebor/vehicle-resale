using AutoMapper;
using MediatR;
using VehicleResale.Application.Commands;
using VehicleResale.Application.DTOs;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class UpdateVehicleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<UpdateVehicleCommand, VehicleDto>
    {
        public async Task<VehicleDto> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await unitOfWork.Vehicles.GetByIdAsync(request.Id);
            
            if (vehicle == null)
                throw new InvalidOperationException($"Veículo com o ID [{request.Id}] não encontrado");

            vehicle.UpdateDetails(
                request.Brand,
                request.Model,
                request.Year,
                request.Color,
                request.Price
            );

            await unitOfWork.Vehicles.UpdateAsync(vehicle);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<VehicleDto>(vehicle);
        }
    }
}