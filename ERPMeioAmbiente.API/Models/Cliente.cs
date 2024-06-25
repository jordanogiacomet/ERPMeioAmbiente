using ERPMeioAmbiente.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Models
{
    public class Cliente
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Contato { get; set; }
        [Required]
        public string CNPJ { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        public string CEP { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        public virtual ICollection<Coleta> Coletas { get; set; } // Collection of Coletas
    }
}
