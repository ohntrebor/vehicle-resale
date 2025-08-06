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
    public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, VehicleDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateVehicleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = new Vehicle(
                request.Brand,
                request.Model,
                request.Year,
                request.Color,
                request.Price
            );

            await _unitOfWork.Vehicles.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleDto>(vehicle);
        }
    }
}