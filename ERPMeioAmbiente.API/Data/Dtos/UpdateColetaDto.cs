using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class UpdateColetaDto
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
        [Required]
        public string ClienteId { get; set; } // Assuming ClienteId is a string in the DTO
    }
}