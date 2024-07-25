using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Verificar existência do cliente
                    var cliente = await _context.Clientes.FindAsync(coletaDto.ClienteId);
                    if (cliente == null)
                        throw new ArgumentException("Cliente não encontrado.");

                    // Verificar existência dos resíduos
                    var residuosExistentes = await _context.Residuos.Where(r => coletaDto.ResiduoIds.Contains(r.Id)).ToListAsync();
                    if (residuosExistentes.Count != coletaDto.ResiduoIds.Count)
                        throw new ArgumentException("Um ou mais resíduos não encontrados.");

                    var coleta = _mapper.Map<Coleta>(coletaDto);
                    coleta.Cliente = cliente;
                    coleta.ColetaResiduos = residuosExistentes.Select(r => new ColetaResiduo { ResiduoId = r.Id }).ToList();

                    _context.Coletas.Add(coleta);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return coleta;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<List<ReadColetaDto>> GetAllColetasAsync(int skip, int take)
        {
            var coletas = await _context.Coletas
                .Include(c => c.Cliente)
                .Include(c => c.ColetaResiduos).ThenInclude(cr => cr.Residuo)
                .Include(c => c.Agendamento)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<List<ReadColetaDto>>(coletas);
        }

        public async Task<ReadColetaDto> GetColetaByIdAsync(int id)
        {
            var coleta = await _context.Coletas
                .Include(c => c.Cliente)
                .Include(c => c.ColetaResiduos).ThenInclude(cr => cr.Residuo)
                .Include(c => c.Agendamento)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (coleta == null) return null;

            return _mapper.Map<ReadColetaDto>(coleta);
        }

        public async Task<bool> UpdateColetaAsync(int id, UpdateColetaDto coletaDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var coleta = await _context.Coletas
                        .Include(c => c.ColetaResiduos)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (coleta == null) return false;

                   
                    _mapper.Map(coletaDto, coleta);

                    var cliente = await _context.Clientes.FindAsync(coletaDto.ClienteId);
                    if (cliente == null)
                        throw new ArgumentException("Cliente não encontrado.");
                    coleta.Cliente = cliente;

                   
                    coleta.ColetaResiduos.Clear();
                    var residuos = await _context.Residuos.Where(r => coletaDto.ResiduoIds.Contains(r.Id)).ToListAsync();
                    coleta.ColetaResiduos = residuos.Select(r => new ColetaResiduo { ColetaId = coleta.Id, ResiduoId = r.Id }).ToList();

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteColetaAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var coleta = await _context.Coletas.FirstOrDefaultAsync(c => c.Id == id);
                    if (coleta == null) return false;

                    _context.Coletas.Remove(coleta);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
