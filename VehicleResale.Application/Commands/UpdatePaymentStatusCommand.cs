using MediatR;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class UpdatePaymentStatusCommand(PaymentWebhookDto dto) : IRequest<bool>
    {
        public string PaymentCode { get; set; } = dto.PaymentCode;
        public string Status { get; set; } = dto.Status;
    }
}