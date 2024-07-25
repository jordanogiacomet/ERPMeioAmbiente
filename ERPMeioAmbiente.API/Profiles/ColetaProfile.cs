using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using System.Linq;

public class ColetaProfile : Profile
{
    public ColetaProfile()
    {
    
        CreateMap<CreateColetaDto, Coleta>()
            .ForMember(dest => dest.ColetaResiduos, opt =>
                opt.MapFrom(src => src.ResiduoIds.Select(id => new ColetaResiduo { ResiduoId = id })));

        CreateMap<CreateColetaForClienteDto, Coleta>()
            .ForMember(dest => dest.ColetaResiduos, opt =>
                opt.MapFrom(src => src.ResiduoIds.Select(id => new ColetaResiduo { ResiduoId = id })));

        CreateMap<UpdateColetaDto, Coleta>()
            .ForMember(dest => dest.ColetaResiduos, opt =>
            {
                opt.PreCondition((src, dest) => src.ResiduoIds != null);
                opt.MapFrom(src => src.ResiduoIds.Select(id => new ColetaResiduo { ResiduoId = id }));
            });

      
        CreateMap<Coleta, ReadColetaDto>()
            .ForMember(dest => dest.Residuos, opt =>
                opt.MapFrom(src => src.ColetaResiduos.Select(cr => cr.Residuo)))
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => new ClienteBasicInfoDto { Id = src.ClienteId }))
            .ForMember(dest => dest.Agendamento, opt => opt.MapFrom(src => src.Agendamento != null ? new AgendamentoBasicInfoDto { Id = src.Agendamento.Id, HorarioChegada = src.Agendamento.HorarioChegada } : null));

        CreateMap<Residuo, ReadResiduoDto>();
    }
}
