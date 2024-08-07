﻿using ERPMeioAmbiente.API.Data.Dtos;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadClienteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Contato { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public List<ColetaBasicInfoDto> Coletas { get; set; } = new List<ColetaBasicInfoDto>();
    }
}
