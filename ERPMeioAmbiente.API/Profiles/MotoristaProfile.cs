using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
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
                .ForMember(dest => dest.Veiculo, opt => opt.MapFrom(src => new VeiculoBasicInfoDto { Id = src.Veiculo.Id, Placa = src.Veiculo.Placa }))
                .ForMember(dest => dest.Agendamentos, opt => opt.MapFrom(src => src.Agendamentos.Select(a => new AgendamentoBasicInfoDto { Id = a.Id, HorarioChegada = a.HorarioChegada })));
            CreateMap<UpdateMotoristaDto, Motorista>();
            CreateMap<Motorista, MotoristaBasicInfoDto>();
        }
    }
}
