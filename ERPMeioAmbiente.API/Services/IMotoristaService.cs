using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPMeioAmbiente.API.Services
{
    public interface IMotoristaService
    {
        Task<Motorista> AddMotoristaAsync(CreateMotoristaDto motoristaDto);
        Task<List<ReadMotoristaDto>> GetAllMotoristasAsync(int skip, int take);
        Task<ReadMotoristaDto> GetMotoristaById(int id);
        Task<bool> UpdateMotoristaAsync(int id, UpdateMotoristaDto motoristaDto);
        Task<bool> DeleteMotoristaAsync(int id);
    }

    public class MotoristaService : IMotoristaService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public MotoristaService(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Motorista> AddMotoristaAsync(CreateMotoristaDto motoristaDto)
        {
            if (motoristaDto.VeiculoId.HasValue)
            {
                var veiculo = await _context.Veiculos.Include(v => v.Motorista).FirstOrDefaultAsync(v => v.Id == motoristaDto.VeiculoId.Value);
                if (veiculo == null) throw new ArgumentException("Veículo não encontrado.");
                if (veiculo.Motorista != null) throw new ArgumentException("Este veículo já está associado a um motorista.");
            }
            var motorista = _mapper.Map<Motorista>(motoristaDto);
            _context.Motoristas.Add(motorista);
            await _context.SaveChangesAsync();
            return motorista;
        }

        public async Task<List<ReadMotoristaDto>> GetAllMotoristasAsync(int skip, int take)
        {
            var motoristas = await _context.Motoristas
                .Include(m => m.Veiculo)
                .Include(m => m.Agendamentos)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return _mapper.Map<List<ReadMotoristaDto>>(motoristas);
        }

        public async Task<ReadMotoristaDto> GetMotoristaById(int id)
        {
            var motorista = await _context.Motoristas
                .Include(m => m.Veiculo)
                .Include(m => m.Agendamentos) // Incluir agendamentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motorista == null) return null;
            return _mapper.Map<ReadMotoristaDto>(motorista);
        }

        public async Task<bool> UpdateMotoristaAsync(int id, UpdateMotoristaDto motoristaDto)
        {
            var motorista = await _context.Motoristas.FindAsync(id);
            if (motorista == null) return false;
            if (motoristaDto.VeiculoId.HasValue)
            {
                var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == motoristaDto.VeiculoId.Value);
                if (!veiculoExiste) throw new ArgumentException("Veículo não encontrado.");
            }
            _mapper.Map(motoristaDto, motorista);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMotoristaAsync(int id)
        {
            var motorista = await _context.Motoristas.FindAsync(id);
            if (motorista == null) return false;
            _context.Motoristas.Remove(motorista);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
