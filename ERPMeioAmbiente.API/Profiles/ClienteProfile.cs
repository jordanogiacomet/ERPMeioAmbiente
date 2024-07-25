using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbienteAPI.Profiles
{
    public class ClienteProfile : AutoMapper.Profile
    {
        public ClienteProfile()
        {
            CreateMap<CreateClienteDto, Cliente>();
            CreateMap<UpdateClienteDto, Cliente>();
            CreateMap<Cliente, ReadClienteDto>()
                .ForMember(dest => dest.Coletas, opt => opt.MapFrom(src => src.Coletas.Select(c => new ColetaBasicInfoDto { Id = c.Id, NumeroVolume = c.NumeroVolume})));
        }
    }
}
