using ERPMeioAmbienteAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPMeioAmbiente.API.Models
{
    public class Coleta
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O número de volumes é obrigatório")]
        public int NumeroVolume { get; set; } // Nome mais descritivo

        [Required(ErrorMessage = "O peso total é obrigatório")]
        public double PesoTotal { get; set; } // Nome mais descritivo e tipo ajustado

        [Required(ErrorMessage = "As dimensões são obrigatórias")]
        public string Dimensoes { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        public string Endereco { get; set; }

        [Required]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        public virtual ICollection<ColetaResiduo> ColetaResiduos { get; set; } = new List<ColetaResiduo>();

        public virtual Agendamento Agendamento { get; set; }
    }
}
