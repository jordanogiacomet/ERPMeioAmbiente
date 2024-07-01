using System;
using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class CreateAgendamentoDto
    {
        [Required]
        public int ColetaId { get; set; }
        [Required]
        public string Motorista { get; set; }
        [Required]
        public string Veiculo { get; set; }
        [Required]
        public string TipoVeiculo { get; set; }
        [Required]
        public DateTime HorarioChegada { get; set; }
    }
}
