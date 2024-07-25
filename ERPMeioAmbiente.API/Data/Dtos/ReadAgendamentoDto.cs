using System;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadAgendamentoDto
    {
        public int Id { get; set; }
        public DateTime HorarioChegada { get; set; }
        public MotoristaBasicInfoDto Motorista { get; set; }
    }
}
