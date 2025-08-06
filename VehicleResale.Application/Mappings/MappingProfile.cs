using AutoMapper;
using VehicleResale.Application.DTOs;
using VehicleResale.Domain.Entities;

namespace VehicleResale.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Vehicle, VehicleDto>();
            CreateMap<Vehicle, VehicleSaleDto>()
                .ForMember(dest => dest.PaymentStatus, 
                    opt => opt.MapFrom(src => src.PaymentStatus.ToString()));
        }
    }
}