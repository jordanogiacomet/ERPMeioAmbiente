using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

namespace ERPMeioAmbienteAPI.Profiles
{
    public class ProdutoProfile : AutoMapper.Profile
    {
        public ProdutoProfile()
        {
            CreateMap<CreateProdutoDto, Produto>();
            CreateMap<UpdateProdutoDto, Produto>();
            CreateMap<Produto, ReadProdutoDto>();
        }
    }
}
