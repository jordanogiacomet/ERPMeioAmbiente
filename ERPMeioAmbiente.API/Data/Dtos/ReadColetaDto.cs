using System.Collections.Generic;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadColetaDto
    {
        public int Id { get; set; }
        public string Nvol { get; set; }
        public string Peso { get; set; }
        public string Dimensoes { get; set; }
        public string Endereco { get; set; }
        public List<ReadResiduoDto> Residuos { get; set; } // Lista de resíduos
    }
}