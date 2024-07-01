using ERPMeioAmbiente.API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPMeioAmbienteAPI.Models
{
    public class Agendamento
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ColetaId { get; set; }
        [ForeignKey("ColetaId")]
        public virtual Coleta Coleta { get; set; }

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
