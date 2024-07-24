using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbiente.API.Profiles
{
    public class MotoristaProfile : Profile
    {
        public MotoristaProfile() 
        {
            CreateMap<CreateMotoristaDto, Motorista>();
            CreateMap<Motorista, ReadMotoristaDto>()
                .ForMember(dest => dest.Veiculo, opt => opt.MapFrom(src => src.Veiculo))
                .ForMember(dest => dest.Agendamentos, opt => opt.MapFrom(src => src.Agendamentos));
            CreateMap<UpdateMotoristaDto, Motorista>();
            CreateMap<Motorista, MotoristaBasicInfoDto>();
        }
    }
}
