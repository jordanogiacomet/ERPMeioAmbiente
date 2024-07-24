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

        public List<ReadResiduoDto> Residuos { get; set; } = new List<ReadResiduoDto>(); // Lista de resíduos

        public ReadClienteDto Cliente { get; set; } // Informações do cliente

        public ReadAgendamentoDto Agendamento { get; set; } // Informação do agendamento associado
    }
}
