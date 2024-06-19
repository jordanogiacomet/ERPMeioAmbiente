using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class UpdateClienteDto
    {
        [Required(ErrorMessage = "O nome do usuário é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo de contato é obrigatório")]
        public string Contato { get; set; }
        [Required(ErrorMessage = "O campo de CNPJ é obrigatório")]
        public string CNPJ { get; set; }
        [Required(ErrorMessage = "O campo de endereço é obrigatório")]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "O campo de CEP é obrigatório")]
        public string CEP { get; set; }
    }
}
