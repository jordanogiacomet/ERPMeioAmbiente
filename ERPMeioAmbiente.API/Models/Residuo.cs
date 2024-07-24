using ERPMeioAmbiente.API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Models
{
    public class Residuo
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome do resíduo é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A categoria é obrigatória")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "O tipo do resíduo é obrigatório")]
        public string Tipo { get; set; }
        public virtual ICollection<ColetaResiduo> ColetaResiduos { get; set; } 
    }
}
