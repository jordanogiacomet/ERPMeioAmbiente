using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbiente.Shared
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Contato { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 14)]
        public string CNPJ { get; set; }

        [Required]
        [StringLength(200)]
        public string Endereco { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 8)]
        public string CEP { get; set; }
    }
}