using System;
using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class CreateAgendamentoDto
    {
        [Required]
        public int ColetaId { get; set; }

        [Required]
        public int MotoristaId { get; set; }

        [Required(ErrorMessage = "O horário de chegada é obrigatório")]
        public DateTime HorarioChegada { get; set; }
    }
}
