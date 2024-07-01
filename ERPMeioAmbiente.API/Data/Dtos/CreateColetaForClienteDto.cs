using System.Collections.Generic;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class CreateColetaForClienteDto
    {
        public string Nvol { get; set; }
        public string Peso { get; set; }
        public string Dimensoes { get; set; }
        public string Endereco { get; set; }
        public List<int> ResiduoIds { get; set; } // Lista de IDs de resíduos
    }
}