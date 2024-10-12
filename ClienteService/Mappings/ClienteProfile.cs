using AutoMapper;
using ClienteService.Domain.Entities;
using ClienteService.DTOs;

namespace ClienteService.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<Cliente, ClienteDto>().ReverseMap();
        }
    }
}
