using AutoMapper;
using ClienteService.Domain.Entities;
using ClienteService.DTOs;
using CuentaService.DTOs;

namespace ClienteService.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteCreateDto, Cliente>();
            CreateMap<ClienteUpdateDto, Cliente>();
            CreateMap<Cliente, ClienteInfoDto>(); 
            CreateMap<ClienteInfoDto, Cliente>();
            CreateMap<Cliente, ClienteDto>().ReverseMap();
        }
    }

}
