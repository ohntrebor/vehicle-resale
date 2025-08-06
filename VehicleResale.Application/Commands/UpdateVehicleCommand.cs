using MediatR;
using System;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class UpdateVehicleCommand : IRequest<VehicleDto>
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }

        public UpdateVehicleCommand(UpdateVehicleDto dto)
        {
            Id = dto.Id;
            Brand = dto.Brand;
            Model = dto.Model;
            Year = dto.Year;
            Color = dto.Color;
            Price = dto.Price;
        }
    }
}