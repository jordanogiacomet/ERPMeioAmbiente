using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadProdutoDto
    {
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
    }
}
