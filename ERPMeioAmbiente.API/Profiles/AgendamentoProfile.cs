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
            CreateMap<UpdateAgendamentoDto, Agendamento>();

            CreateMap<Agendamento, ReadAgendamentoDto>()
                .ForMember(dest => dest.Motorista, opt => opt.MapFrom(src => src.Motorista != null ? new MotoristaBasicInfoDto { Id = src.MotoristaId, Nome = src.Motorista.Nome } : null));

            CreateMap<Agendamento, AgendamentoBasicInfoDto>();
        }
    }
}
