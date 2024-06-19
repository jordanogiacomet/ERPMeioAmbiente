using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class CreateClienteDto
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo de contato é obrigatório")]
        public string Contato { get; set; }
        [Required(ErrorMessage = "O CNPJ do cliente é obrigatório")]
        public string CNPJ { get; set; }
        [Required(ErrorMessage = "O endereço do cliente é obrigatório")]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "O CEP do cliente é obrigatório")]
        public string CEP { get; set; }
    }
}
