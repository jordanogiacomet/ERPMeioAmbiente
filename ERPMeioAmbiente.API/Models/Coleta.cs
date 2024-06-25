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
        [Required]
        public string Nvol { get; set; }
        [Required]
        public string Peso { get; set; }
        [Required]
        public string Dimensoes { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        public string TipoResiduo { get; set; }

        [Required]
        public int ClienteId { get; set; } // Adjusted to int
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }
    }
}
