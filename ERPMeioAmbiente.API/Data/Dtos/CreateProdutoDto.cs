using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class CreateProdutoDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O categoria é obrigatória")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "O tipo do produto é obrigatório")]
        public string Tipo { get; set; }
    }
}
