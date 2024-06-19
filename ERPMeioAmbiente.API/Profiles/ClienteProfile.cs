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
            CreateMap<Cliente, ReadClienteDto>();
        }
    }
}
