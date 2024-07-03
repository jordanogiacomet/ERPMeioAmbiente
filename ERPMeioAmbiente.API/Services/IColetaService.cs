using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Services
{
    public interface IColetaService
    {
        Task<Coleta> AddColetaAsync(CreateColetaDto coletaDto);
        Task<List<ReadColetaDto>> GetAllColetasAsync(int skip, int take);
        Task<ReadColetaDto> GetColetaByIdAsync(int id);
        Task<bool> UpdateColetaAsync(int id, UpdateColetaDto coletaDto);
        Task<bool> DeleteColetaAsync(int id);
    }

    public class ColetaService : IColetaService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public ColetaService(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Coleta> AddColetaAsync(CreateColetaDto coletaDto)
        {
            var coleta = _mapper.Map<Coleta>(coletaDto);
            _context.Coletas.Add(coleta);
            await _context.SaveChangesAsync();
            return coleta;
        }

        public async Task<List<ReadColetaDto>> GetAllColetasAsync(int skip, int take)
        {
            var coletas = await _context.Coletas
                .Include(c => c.Cliente)
                .Include(c => c.ColetaResiduos)
                .ThenInclude(cr => cr.Residuo)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<List<ReadColetaDto>>(coletas);
        }

        public async Task<ReadColetaDto> GetColetaByIdAsync(int id)
        {
            var coleta = await _context.Coletas
                .Include(c => c.ColetaResiduos)
                .ThenInclude(cr => cr.Residuo)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (coleta == null)
            {
                return null;
            }

            return _mapper.Map<ReadColetaDto>(coleta);
        }

        public async Task<bool> UpdateColetaAsync(int id, UpdateColetaDto coletaDto)
        {
            var coleta = await _context.Coletas
                .Include(c => c.ColetaResiduos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (coleta == null) return false;

            _mapper.Map(coletaDto, coleta);

            // Atualizar a relação muitos-para-muitos
            var existingResiduoIds = coleta.ColetaResiduos.Select(cr => cr.ResiduoId).ToList();
            var newResiduoIds = coletaDto.ResiduoIds.Except(existingResiduoIds).ToList();
            var removedResiduoIds = existingResiduoIds.Except(coletaDto.ResiduoIds).ToList();

            // Adicionar novos resíduos
            foreach (var residuoId in newResiduoIds)
            {
                var residuo = await _context.Residuos.FindAsync(residuoId);
                if (residuo != null)
                {
                    coleta.ColetaResiduos.Add(new ColetaResiduo { ColetaId = coleta.Id, ResiduoId = residuoId });
                }
            }

            // Remover resíduos antigos
            foreach (var residuoId in removedResiduoIds)
            {
                var residuoToRemove = coleta.ColetaResiduos.FirstOrDefault(cr => cr.ResiduoId == residuoId);
                if (residuoToRemove != null)
                {
                    _context.ColetaResiduos.Remove(residuoToRemove);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteColetaAsync(int id)
        {
            var coleta = await _context.Coletas.FirstOrDefaultAsync(c => c.Id == id);
            if (coleta == null)
            {
                Console.WriteLine($"Coleta com ID {id} não encontrada."); // Adicione uma mensagem de depuração
                return false;
            }

            _context.Coletas.Remove(coleta);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Erro ao deletar a coleta com ID {id}: {ex.Message}");
                return false;
            }
        }
    }
}
