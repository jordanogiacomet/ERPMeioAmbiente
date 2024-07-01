using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbienteAPI.Profiles
{
    public class ResiduoProfile : Profile
    {
        public ResiduoProfile()
        {
            CreateMap<CreateResiduoDto, Residuo>();
            CreateMap<UpdateResiduoDto, Residuo>();
            CreateMap<Residuo, ReadResiduoDto>();
        }
    }
}