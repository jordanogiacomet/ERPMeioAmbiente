using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Services
{
    public interface IAgendamentoService
    {
        Task<Agendamento> AddAgendamentoAsync(CreateAgendamentoDto agendamentoDto);
        Task<List<ReadAgendamentoDto>> GetAllAgendamentosAsync(int skip, int take);
        Task<ReadAgendamentoDto> GetAgendamentoByIdAsync(int id);
        Task<bool> UpdateAgendamentoAsync(int id, UpdateAgendamentoDto agendamentoDto);
        Task<bool> DeleteAgendamentoAsync(int id);
    }

    public class AgendamentoService : IAgendamentoService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public AgendamentoService(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Agendamento> AddAgendamentoAsync(CreateAgendamentoDto agendamentoDto)
        {
            Agendamento agendamento = _mapper.Map<Agendamento>(agendamentoDto);
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();
            return agendamento;
        }

        public async Task<List<ReadAgendamentoDto>> GetAllAgendamentosAsync(int skip, int take)
        {
            var agendamentos = await _context.Agendamentos.Skip(skip).Take(take).ToListAsync();
            return _mapper.Map<List<ReadAgendamentoDto>>(agendamentos);
        }

        public async Task<ReadAgendamentoDto> GetAgendamentoByIdAsync(int id)
        {
            var agendamento = await _context.Agendamentos.FirstOrDefaultAsync(a => a.Id == id);
            if (agendamento == null)
            {
                return null;
            }
            return _mapper.Map<ReadAgendamentoDto>(agendamento);
        }

        public async Task<bool> UpdateAgendamentoAsync(int id, UpdateAgendamentoDto agendamentoDto)
        {
            var agendamento = await _context.Agendamentos.FirstOrDefaultAsync(a => a.Id == id);
            if (agendamento == null)
            {
                return false;
            }

            var coleta = await _context.Coletas.FirstOrDefaultAsync(c => c.Id == agendamentoDto.ColetaId);
            if (coleta == null)
            {
                return false;
            }

            _mapper.Map(agendamentoDto, agendamento);
            agendamento.Coleta = coleta; // Associar a coleta corretamente

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAgendamentoAsync(int id)
        {
            var agendamento = await _context.Agendamentos.FirstOrDefaultAsync(a => a.Id == id);
            if (agendamento == null)
            {
                return false;
            }

            _context.Remove(agendamento);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
