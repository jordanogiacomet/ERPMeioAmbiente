using System.Collections.Generic;
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
        public List<int> ResiduoIds { get; set; } // Lista de IDs de resíduos
    }
}
