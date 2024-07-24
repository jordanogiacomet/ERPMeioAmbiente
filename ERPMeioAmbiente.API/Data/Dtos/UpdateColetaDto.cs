using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class UpdateColetaDto
    {
        [Required(ErrorMessage = "O número de volumes é obrigatório")]
        public int NumeroVolume { get; set; }

        [Required(ErrorMessage = "O peso total é obrigatório")]
        [Range(0.1, double.MaxValue, ErrorMessage = "O peso total deve ser maior que zero")]
        public double PesoTotal { get; set; }

        [Required(ErrorMessage = "As dimensões são obrigatórias")]
        [RegularExpression(@"^\d+x\d+x\d+$", ErrorMessage = "As dimensões devem estar no formato 'Comprimento x Largura x Altura'")]
        public string Dimensoes { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        public string Endereco { get; set; }

        public List<int> ResiduoIds { get; set; } = new List<int>(); // Lista de IDs de resíduos
    }
}
