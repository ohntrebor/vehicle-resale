using MediatR;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class UpdateVehicleCommand(UpdateVehicleDto dto) : IRequest<VehicleDto>
    {
        public Guid Id { get; set; } = dto.Id;
        public string Brand { get; set; } = dto.Brand;
        public string Model { get; set; } = dto.Model;
        public int Year { get; set; } = dto.Year;
        public string Color { get; set; } = dto.Color;
        public decimal Price { get; set; } = dto.Price;
    }
}