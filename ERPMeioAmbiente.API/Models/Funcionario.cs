using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Models
{
    public class Funcionario
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage= "O nome de funcionário é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O email de funcionário é obrigatório")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O telefone de usuário é obrigátorio")]
        public string Telefone { get; set; }
    }
}
