using MediatR;
using VehicleResale.Application.Commands;
using VehicleResale.Domain.Enums;
using VehicleResale.Domain.Interfaces;

namespace VehicleResale.Application.Handlers
{
    public class UpdatePaymentStatusHandler : IRequestHandler<UpdatePaymentStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePaymentStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByPaymentCodeAsync(request.PaymentCode);
            
            if (vehicle == null)
                return false;

            var status = request.Status.ToLower() switch
            {
                "confirmed" => PaymentStatus.Confirmed,
                "cancelled" => PaymentStatus.Cancelled,
                _ => throw new InvalidOperationException($"Invalid payment status: {request.Status}")
            };

            vehicle.UpdatePaymentStatus(status);
            
            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}