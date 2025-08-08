using MediatR;
using VehicleResale.Application.Commands;
using VehicleResale.Domain.Enums;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class UpdatePaymentStatusHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdatePaymentStatusCommand, bool>
    {
        public async Task<bool> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await unitOfWork.Vehicles.GetByPaymentCodeAsync(request.PaymentCode);
            
            if (vehicle == null)
                return false;

            var status = request.Status.ToLower() switch
            {
                "confirmed" => PaymentStatus.Confirmed,
                "cancelled" => PaymentStatus.Cancelled,
                _ => throw new InvalidOperationException($"Status de pagamento inválido: [{request.Status}]. Deve ser 'confirmed' ou 'cancelled'.")
            };

            vehicle.UpdatePaymentStatus(status);
            
            await unitOfWork.Vehicles.UpdateAsync(vehicle);
            await unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}