using System;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadAgendamentoDto
    {
        public int Id { get; set; }
        public int ColetaId { get; set; }
        public ReadColetaDto Coleta { get; set; }
        public int MotoristaId { get; set; }
        public ReadMotoristaDto Motorista { get; set; }
        public DateTime HorarioChegada { get; set; }
    }
}
