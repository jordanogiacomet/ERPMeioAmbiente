using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class UpdateResiduoDto
    {
        [Required(ErrorMessage = "O nome do resíduo é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A categoria do resíduo obrigatória")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "O tipo do resíduo é obrigatório")]
        public string Tipo { get; set; }
    }
}
