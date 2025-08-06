using MediatR;
using System;
using VehicleResale.Application.DTOs;

namespace VehicleResale.Application.Commands
{
    public class CreateVehicleCommand : IRequest<VehicleDto>
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }

        public CreateVehicleCommand(CreateVehicleDto dto)
        {
            Brand = dto.Brand;
            Model = dto.Model;
            Year = dto.Year;
            Color = dto.Color;
            Price = dto.Price;
        }
    }
}