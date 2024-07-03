using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                    throw new System.Exception("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<List<ReadClienteDto>> GetAllClientesAsync(int skip, int take)
        {
            var clientes = await _context.Clientes.Skip(skip).Take(take).ToListAsync();
            return _mapper.Map<List<ReadClienteDto>>(clientes);
        }

        public async Task<ReadClienteDto> GetClienteByIdAsync(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == id);
            if (cliente == null)
            {
                return null;
            }
            return _mapper.Map<ReadClienteDto>(cliente);
        }

        public async Task<ReadClienteDto> GetClienteAtualAsync(string userId)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return null;
            }
            return _mapper.Map<ReadClienteDto>(cliente);
        }

        public async Task<ReadClienteDto> UpdateClienteAtualAsync(string userId, UpdateClienteDto clienteDto)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return null;
            }

            _mapper.Map(clienteDto, cliente);
            await _context.SaveChangesAsync();
            return _mapper.Map<ReadClienteDto>(cliente);
        }

        public async Task<bool> UpdateClienteAsync(int id, UpdateClienteDto clienteDto)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return false;

            _mapper.Map(clienteDto, cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cliente = await _context.Clientes.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
                    if (cliente == null)
                    {
                        return false;
                    }

                    var user = await _userManager.FindByIdAsync(cliente.UserId);
                    if (user == null)
                    {
                        return false;
                    }

                    _context.Clientes.Remove(cliente);
                    var userResult = await _userManager.DeleteAsync(user);
                    if (!userResult.Succeeded)
                    {
                        throw new System.Exception(string.Join(", ", userResult.Errors.Select(e => e.Description)));
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<ReadColetaDto> SolicitaColetaAsync(string userId, CreateColetaForClienteDto coletaDto)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return null;
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

            return _mapper.Map<ReadColetaDto>(coleta);
        }

        public async Task<List<ReadColetaDto>> GetColetasDoClienteAsync(string userId)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return null;
            }

            var coletas = await _context.Coletas
                .Include(c => c.ColetaResiduos)
                .ThenInclude(cr => cr.Residuo)
                .Where(c => c.ClienteId == cliente.Id)
                .ToListAsync();

            return _mapper.Map<List<ReadColetaDto>>(coletas);
        }

        public async Task<ReadColetaDto> GetColetaDoClientePorIdAsync(string userId, int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return null;
            }

            var coleta = await _context.Coletas
                .Include(c => c.ColetaResiduos)
                .ThenInclude(cr => cr.Residuo)
                .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == cliente.Id);
            if (coleta == null)
            {
                return null;
            }

            return _mapper.Map<ReadColetaDto>(coleta);
        }

        public async Task<bool> UpdateColetaDoClienteAsync(string userId, int id, UpdateColetaDto coletaDto)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return false;
            }

            var coleta = await _context.Coletas
                .Include(c => c.ColetaResiduos)
                .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == cliente.Id);
            if (coleta == null)
            {
                return false;
            }

            _mapper.Map(coletaDto, coleta);

            coleta.ColetaResiduos.Clear();
            foreach (var residuoId in coletaDto.ResiduoIds)
            {
                var residuo = await _context.Residuos.FindAsync(residuoId);
                if (residuo == null)
                {
                    return false;
                }
                coleta.ColetaResiduos.Add(new ColetaResiduo { ResiduoId = residuoId, Residuo = residuo });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteColetaDoClienteAsync(string userId, int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cliente == null)
            {
                return false;
            }

            var coleta = await _context.Coletas.FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == cliente.Id);
            if (coleta == null)
            {
                return false;
            }

            _context.Remove(coleta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
