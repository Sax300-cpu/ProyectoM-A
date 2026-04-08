using AutoMapper;
using VentasService.Models;
using VentasService.DTOs;

namespace VentasService.Profiles
{
    public class DetalleVentaProfile : Profile
    {
        public DetalleVentaProfile()
        {
            // 🔹 De entidad a DTO (lectura)
            CreateMap<DetalleVenta, DetalleVentaDto>();

            // 🔹 De CreateDto a entidad (creación)
            CreateMap<DetalleVentaCreateDto, DetalleVenta>()
                .AfterMap((src, dest) => dest.CalcularSubtotal());

            // 🔹 De UpdateDto a entidad (actualización)
            CreateMap<DetalleVentaUpdateDto, DetalleVenta>()
                .AfterMap((src, dest) => dest.CalcularSubtotal());

            // 🔹 De entidad a UpdateDto (opcional, útil si quieres devolver datos al editar)
            CreateMap<DetalleVenta, DetalleVentaUpdateDto>();
        }
    }
}
