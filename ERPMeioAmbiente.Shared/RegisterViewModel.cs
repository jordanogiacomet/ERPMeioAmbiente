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
        public string Role { get; set; } // Novo campo para indicar o papel do usuário
    }
}
