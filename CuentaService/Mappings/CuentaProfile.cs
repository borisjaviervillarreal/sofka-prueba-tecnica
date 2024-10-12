using AutoMapper;
using CuentaService.Domain.Entities;
using CuentaService.DTOs;

namespace CuentaService.Mappings
{
    public class CuentaProfile : Profile
    {
        public CuentaProfile()
        {
            CreateMap<Cuenta, CuentaDto>().ReverseMap();
            CreateMap<Cuenta, CuentaCreateDto>().ReverseMap();
            CreateMap<Cuenta, CuentaUpdateDto>().ReverseMap();

            CreateMap<Movimiento, MovimientoDto>().ReverseMap();
            CreateMap<MovimientoCreateDto, Movimiento>()
                .ForMember(dest => dest.Fecha, opt => opt.Ignore());
        }
    }

}
