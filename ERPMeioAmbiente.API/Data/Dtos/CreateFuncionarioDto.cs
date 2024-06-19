using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbiente.API.Data.Dtos
{
    public class CreateFuncionarioDto
    {
        [Required(ErrorMessage = "O nome de funcionário é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O email de funcionário é obrigatório")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O telefone de usuário é obrigátorio")]
        public string Telefone { get; set; }
    }
}
