﻿using ERPMeioAmbiente.API.Data.Dtos;
using System;
using System.Collections.Generic;

namespace ERPMeioAmbienteAPI.Data.Dtos
{
    public class ReadColetaDto
    {
        public int Id { get; set; }
        public int NumeroVolume { get; set; }
        public double PesoTotal { get; set; }
        public string Dimensoes { get; set; }
        public string Endereco { get; set; }
        public int ClienteId { get; set; }
        public ClienteBasicInfoDto Cliente { get; set; }
        public AgendamentoBasicInfoDto Agendamento { get; set; }
        public List<ReadResiduoDto> Residuos { get; set; } = new List<ReadResiduoDto>();
    }
}
