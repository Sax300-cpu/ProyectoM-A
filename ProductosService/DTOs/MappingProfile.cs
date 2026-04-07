using AutoMapper;
using ProductosService.Models;

namespace ProductosService.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoCreateDto, Producto>()
                .ForMember(dest => dest.ProductoUUID, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<ProductoUpdateDto, Producto>()
                .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}