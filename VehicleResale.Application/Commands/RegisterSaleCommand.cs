using MediatR;
using System;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class RegisterSaleCommand : IRequest<VehicleSaleDto>
    {
        public Guid VehicleId { get; set; }
        public string BuyerCpf { get; set; }

        public RegisterSaleCommand(RegisterSaleDto dto)
        {
            VehicleId = dto.VehicleId;
            BuyerCpf = dto.BuyerCpf;
        }
    }
}