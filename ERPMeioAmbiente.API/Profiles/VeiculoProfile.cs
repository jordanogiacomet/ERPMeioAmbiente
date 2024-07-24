using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbiente.API.Profiles
{
    public class VeiculoProfile : Profile
    {
        public VeiculoProfile()
        {
            CreateMap<CreateVeiculoDto, Veiculo>();
            CreateMap<Veiculo, ReadVeiculoDto>()
                .ForMember(dest => dest.Motorista, opt => opt.MapFrom(src => src.Motorista));
            CreateMap<UpdateVeiculoDto, Veiculo>();
            CreateMap<Veiculo, VeiculoBasicInfoDto>();
        }
    }
}
