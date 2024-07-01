using ERPMeioAmbienteAPI.Data.Dtos;
using System;

namespace ERPMeioAmbiente.API.Data.Dtos
{
    public class ReadAgendamentoDto
    {
        public int Id { get; set; }
        public int ColetaId { get; set; }
        public ReadColetaDto Coleta { get; set; }
        public string Motorista { get; set; }
        public string Veiculo { get; set; }
        public string TipoVeiculo { get; set; }
        public DateTime HorarioChegada { get; set; }
    }
}
