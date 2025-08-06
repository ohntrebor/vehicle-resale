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
    public class RegisterSaleHandler : IRequestHandler<RegisterSaleCommand, VehicleSaleDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegisterSaleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<VehicleSaleDto> Handle(RegisterSaleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
            
            if (vehicle == null)
                throw new InvalidOperationException($"Vehicle with ID {request.VehicleId} not found");

            // Gerar código de pagamento único
            var paymentCode = $"PAY-{Guid.NewGuid():N}".Substring(0, 20);
            
            vehicle.RegisterSale(request.BuyerCpf, paymentCode);
            
            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleSaleDto>(vehicle);
        }
    }
}