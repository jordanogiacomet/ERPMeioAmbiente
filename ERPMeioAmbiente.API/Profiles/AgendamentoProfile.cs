using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbiente.API.Profiles
{
    public class AgendamentoProfile : Profile
    {
        public AgendamentoProfile()
        {
            
            CreateMap<CreateAgendamentoDto, Agendamento>();
            CreateMap<Agendamento, ReadAgendamentoDto>()
                .ForMember(dest => dest.Coleta, opt => opt.MapFrom(src => src.Coleta))
                .ForMember(dest => dest.Motorista, opt => opt.MapFrom(src => src.Motorista))
                .ForMember(dest => dest.MotoristaId, opt => opt.MapFrom(src => src.MotoristaId))
                .ForMember(dest => dest.ColetaId, opt => opt.MapFrom(src => src.ColetaId));
            CreateMap<UpdateAgendamentoDto, Agendamento>();
            CreateMap<Agendamento, AgendamentoBasicInfoDto>();
        }
    }
}
