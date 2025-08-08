using MediatR;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class RegisterSaleCommand(RegisterSaleDto dto) : IRequest<VehicleSaleDto>
    {
        public Guid VehicleId { get; set; } = dto.VehicleId;
        public string BuyerCpf { get; set; } = dto.BuyerCpf;
    }
}