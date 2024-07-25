using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Services
{
    public interface IClienteService
    {
        Task<Cliente> AddClienteAsync(CreateClienteDto clienteDto);
        Task<List<ReadClienteDto>> GetAllClientesAsync(int skip, int take);
        Task<ReadClienteDto> GetClienteByIdAsync(int id);
        Task<ReadClienteDto> GetClienteAtualAsync(string userId);
        Task<ReadClienteDto> UpdateClienteAtualAsync(string userId, UpdateClienteDto clienteDto);
        Task<bool> UpdateClienteAsync(int id, UpdateClienteDto clienteDto);
        Task<bool> DeleteClienteAsync(int id);
        Task<ReadColetaDto> SolicitaColetaAsync(string userId, CreateColetaForClienteDto coletaDto);
        Task<List<ReadColetaDto>> GetColetasDoClienteAsync(string userId);
        Task<ReadColetaDto> GetColetaDoClientePorIdAsync(string userId, int id);
        Task<bool> UpdateColetaDoClienteAsync(string userId, int id, UpdateColetaDto coletaDto);
        Task<bool> DeleteColetaDoClienteAsync(string userId, int id);
    }

    public class ClienteService : IClienteService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public ClienteService(ERPMeioAmbienteContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Cliente> AddClienteAsync(CreateClienteDto clienteDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = _mapper.Map<Cliente>(clienteDto);

                if (!string.IsNullOrEmpty(clienteDto.Email) && !string.IsNullOrEmpty(clienteDto.Password))
                {
                    var identityUser = new IdentityUser
                    {
                        Email = clienteDto.Email,
                        UserName = clienteDto.Email,
                    };

                    var result = await _userManager.CreateAsync(identityUser, clienteDto.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(identityUser, "Cliente");
                        cliente.UserId = identityUser.Id;
                    }
                    else
                    {
                        throw new Exception("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return cliente;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerExceptionMessage = ex.InnerException?.Message ?? "N/A";
                throw new Exception($"Erro ao adicionar cliente: {ex.Message}. Inner Exception: {innerExceptionMessage}", ex);
            }
        }

        public async Task<List<ReadClienteDto>> GetAllClientesAsync(int skip, int take)
        {
            try
            {
                var clientes = await _context.Clientes.Skip(skip).Take(take).ToListAsync();
                return _mapper.Map<List<ReadClienteDto>>(clientes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao recuperar clientes: {ex.Message}");
            }
        }

        public async Task<ReadClienteDto> GetClienteByIdAsync(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == id);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }
                return _mapper.Map<ReadClienteDto>(cliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao recuperar cliente: {ex.Message}");
            }
        }

        public async Task<ReadClienteDto> GetClienteAtualAsync(string userId)
        {
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }
                return _mapper.Map<ReadClienteDto>(cliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao recuperar cliente atual: {ex.Message}");
            }
        }

        public async Task<ReadClienteDto> UpdateClienteAtualAsync(string userId, UpdateClienteDto clienteDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                _mapper.Map(clienteDto, cliente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return _mapper.Map<ReadClienteDto>(cliente);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao atualizar cliente: {ex.Message}");
            }
        }

        public async Task<bool> UpdateClienteAsync(int id, UpdateClienteDto clienteDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
                if (cliente == null) throw new Exception("Cliente não encontrado.");

                _mapper.Map(clienteDto, cliente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao atualizar cliente: {ex.Message}");
            }
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Clientes.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                var user = await _userManager.FindByIdAsync(cliente.UserId);
                if (user == null)
                {
                    throw new Exception("Usuário não encontrado.");
                }

                _context.Clientes.Remove(cliente);
                var userResult = await _userManager.DeleteAsync(user);
                if (!userResult.Succeeded)
                {
                    throw new Exception(string.Join(", ", userResult.Errors.Select(e => e.Description)));
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao deletar cliente: {ex.Message}");
            }
        }

        public async Task<ReadColetaDto> SolicitaColetaAsync(string userId, CreateColetaForClienteDto coletaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                var coleta = _mapper.Map<Coleta>(coletaDto);
                coleta.ClienteId = cliente.Id;
                coleta.Cliente = cliente;
                coleta.ColetaResiduos = new List<ColetaResiduo>();

                foreach (var residuoId in coletaDto.ResiduoIds)
                {
                    var residuo = await _context.Residuos.FindAsync(residuoId);
                    if (residuo == null)
                    {
                        throw new Exception($"Residuo com ID {residuoId} não encontrado.");
                    }
                    coleta.ColetaResiduos.Add(new ColetaResiduo { ResiduoId = residuoId });
                }

                _context.Coletas.Add(coleta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return _mapper.Map<ReadColetaDto>(coleta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao solicitar coleta: {ex.Message}");
            }
        }

        public async Task<List<ReadColetaDto>> GetColetasDoClienteAsync(string userId)
        {
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                var coletas = await _context.Coletas
                    .Include(c => c.ColetaResiduos)
                    .ThenInclude(cr => cr.Residuo)
                    .Include(c => c.Agendamento)
                    .Where(c => c.ClienteId == cliente.Id)
                    .ToListAsync();

                return _mapper.Map<List<ReadColetaDto>>(coletas);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao recuperar coletas do cliente: {ex.Message}");
            }
        }

        public async Task<ReadColetaDto> GetColetaDoClientePorIdAsync(string userId, int id)
        {
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                var coleta = await _context.Coletas
                    .Include(c => c.ColetaResiduos)
                    .ThenInclude(cr => cr.Residuo)
                    .Include(c => c.Agendamento)
                    .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == cliente.Id);
                if (coleta == null)
                {
                    throw new Exception("Coleta não encontrada.");
                }

                return _mapper.Map<ReadColetaDto>(coleta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao recuperar coleta: {ex.Message}");
            }
        }

        public async Task<bool> UpdateColetaDoClienteAsync(string userId, int id, UpdateColetaDto coletaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                var coleta = await _context.Coletas
                    .Include(c => c.ColetaResiduos)
                    .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == cliente.Id);
                if (coleta == null)
                {
                    throw new Exception("Coleta não encontrada.");
                }

                _mapper.Map(coletaDto, coleta);

                coleta.ColetaResiduos.Clear();
                foreach (var residuoId in coletaDto.ResiduoIds)
                {
                    var residuo = await _context.Residuos.FindAsync(residuoId);
                    if (residuo == null)
                    {
                        throw new Exception($"Residuo com ID {residuoId} não encontrado.");
                    }
                    coleta.ColetaResiduos.Add(new ColetaResiduo { ResiduoId = residuoId, Residuo = residuo });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao atualizar coleta: {ex.Message}");
            }
        }

        public async Task<bool> DeleteColetaDoClienteAsync(string userId, int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                var coleta = await _context.Coletas.FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == cliente.Id);
                if (coleta == null)
                {
                    throw new Exception("Coleta não encontrada.");
                }

                _context.Remove(coleta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao deletar coleta: {ex.Message}");
            }
        }
    }
}
