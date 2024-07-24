namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadVeiculoDto
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public MotoristaBasicInfoDto Motorista { get; set; } // Objeto aninhado simplificado
    }
}
