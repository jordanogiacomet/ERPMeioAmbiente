using ERPMeioAmbiente.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPMeioAmbienteAPI.Models
{
    public class ColetaResiduo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ColetaId { get; set; }
        [ForeignKey("ColetaId")]
        public virtual Coleta Coleta { get; set; }

        [Required]
        public int ResiduoId { get; set; }
        [ForeignKey("ResiduoId")]
        public virtual Residuo Residuo { get; set; }
    }
}
