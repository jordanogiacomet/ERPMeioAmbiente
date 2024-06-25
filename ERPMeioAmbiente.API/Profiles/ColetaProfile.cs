using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;

public class ColetaProfile : Profile
{
    public ColetaProfile()
    {
        CreateMap<CreateColetaForClienteDto, Coleta>();
        CreateMap<CreateColetaDto, Coleta>();
        CreateMap<Coleta, ReadColetaDto>()
            .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente.Nome));
        CreateMap<UpdateColetaDto, Coleta>();
    }
}