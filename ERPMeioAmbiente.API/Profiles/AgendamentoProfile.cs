using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbiente.API.Models;
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
                .ForMember(dest => dest.Coleta, opt => opt.MapFrom(src => src.Coleta));
            CreateMap<UpdateAgendamentoDto, Agendamento>();
        }
    }
}
