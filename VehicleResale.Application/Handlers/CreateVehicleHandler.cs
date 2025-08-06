using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VehicleResale.Application.Commands;
using VehicleResale.Application.DTOs;
using VehicleResale.Domain.Entities;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class CreateVehicleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<CreateVehicleCommand, VehicleDto>
    {
        public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = new Vehicle(
                request.Brand,
                request.Model,
                request.Year,
                request.Color,
                request.Price
            );

            await unitOfWork.Vehicles.AddAsync(vehicle);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<VehicleDto>(vehicle);
        }
    }
}