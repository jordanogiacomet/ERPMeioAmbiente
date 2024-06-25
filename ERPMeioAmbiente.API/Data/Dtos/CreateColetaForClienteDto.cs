using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class CreateColetaForClienteDto
    {
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
    }
}
