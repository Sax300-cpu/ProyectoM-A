using AutoMapper;
using VentasService.Models;
using VentasService.DTOs;

namespace VentasService.Profiles
{
    public class VentaProfile : Profile
    {
        public VentaProfile()
        {
            // De entidad a DTO
            CreateMap<Venta, VentaDto>();

            // De DTO de creación a entidad
            CreateMap<VentaCreateDto, Venta>();

            // De DTO de actualización a entidad
            CreateMap<VentaUpdateDto, Venta>();

            // También puedes mapear de entidad a DTO de actualización si lo necesitas
            CreateMap<Venta, VentaUpdateDto>();
        }
    }
}
