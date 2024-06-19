using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbiente.API.Data.Dtos
{
    public class ReadFuncionarioDto
    {     
        public string Nome { get; set; }
 
        public string Email { get; set; }
 
        public string Telefone { get; set; }
    }
}
