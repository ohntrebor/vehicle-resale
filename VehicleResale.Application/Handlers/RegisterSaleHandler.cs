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
    public class RegisterSaleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<RegisterSaleCommand, VehicleSaleDto>
    {
        public async Task<VehicleSaleDto> Handle(RegisterSaleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
            
            if (vehicle == null)
                throw new InvalidOperationException($"Vehicle with ID {request.VehicleId} not found");

            // Gerar código de pagamento único
            var paymentCode = $"PAY-{Guid.NewGuid():N}".Substring(0, 20);
            
            vehicle.RegisterSale(request.BuyerCpf, paymentCode);
            
            await unitOfWork.Vehicles.UpdateAsync(vehicle);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<VehicleSaleDto>(vehicle);
        }
    }
}