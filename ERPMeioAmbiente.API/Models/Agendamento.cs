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
        public int MotoristaId { get; set; } 
        [ForeignKey("MotoristaId")]
        public virtual Motorista Motorista { get; set; } 

        [Required(ErrorMessage = "O horário de chegada é obrigatório")]
        public DateTime HorarioChegada { get; set; }
    }
}
