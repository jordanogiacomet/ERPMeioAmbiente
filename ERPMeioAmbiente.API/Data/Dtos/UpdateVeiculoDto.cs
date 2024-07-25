using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class UpdateVeiculoDto
    {
        [Required(ErrorMessage = "A placa do veículo é obrigatória")]
        public string Placa { get; set; }
        [Required(ErrorMessage = "O modelo do veículo é obrigatório")]
        public string Modelo { get; set; }
        [Required(ErrorMessage = "A marca do veículo é obrigatória")]
        public string Marca { get; set; }
        public int? MotoristaId { get; set; }
    }
}
