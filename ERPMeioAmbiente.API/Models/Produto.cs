using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Models
{
    public class Produto
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O categoria é obrigatória")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "O tipo do produto é obrigatório")]
        public string Tipo { get; set; }
    }
}
