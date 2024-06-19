using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbiente.API.Profiles
{
    public class FuncionarioProfile : AutoMapper.Profile
    {
        public FuncionarioProfile() 
        {
            CreateMap<CreateFuncionarioDto, Funcionario>();
            CreateMap<UpdateFuncionarioDto, Funcionario>();
            CreateMap<Funcionario, ReadFuncionarioDto>();
        }
    }
}
