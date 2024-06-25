namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadColetaDto
    {
        public int Id { get; set; }
        public string Nvol { get; set; }
        public string Peso { get; set; }
        public string Dimensoes { get; set; }
        public string Endereco { get; set; }
        public string TipoResiduo { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; }
    }
}