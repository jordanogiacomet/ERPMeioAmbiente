using ERPMeioAmbiente.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Models
{
    public class Cliente
    {
        public  int Id { get; set;  }
        public string Nome { get; set; }
        public string Contato { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
        public virtual ICollection<Coleta> Coletas { get; set; }
    }
}
