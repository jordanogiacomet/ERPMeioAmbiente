using ERPMeioAmbiente.API.Data.Dtos;
using System.Collections.Generic;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadMotoristaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NumeroCarteiraHabilitacao { get; set; }
        public string Telefone { get; set; }
        public VeiculoBasicInfoDto Veiculo { get; set; }
        public List<AgendamentoBasicInfoDto> Agendamentos { get; set; } = new List<AgendamentoBasicInfoDto>();
    }
}
