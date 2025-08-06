using MediatR;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class UpdatePaymentStatusCommand : IRequest<bool>
    {
        public string PaymentCode { get; set; }
        public string Status { get; set; }

        public UpdatePaymentStatusCommand(PaymentWebhookDto dto)
        {
            PaymentCode = dto.PaymentCode;
            Status = dto.Status;
        }
    }
}