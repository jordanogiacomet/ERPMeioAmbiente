using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPMeioAmbiente.API.Services
{
    public interface IVeiculoService
    {
        Task<Veiculo> AddVeiculoAsync(CreateVeiculoDto veiculoDto);
        Task<List<ReadVeiculoDto>> GetAllVeiculosAsync(int skip, int take);
        Task<ReadVeiculoDto> GetVeiculoByIdAsync(int id);
        Task<bool> UpdateVeiculoAsync(int id, UpdateVeiculoDto veiculoDto);
        Task<bool> DeleteVeiculoAsync(int id);
    }
    public class VeiculoService : IVeiculoService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public VeiculoService(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Veiculo> AddVeiculoAsync(CreateVeiculoDto veiculoDto)
        {
            if(await _context.Veiculos.AnyAsync(v => v.Placa == veiculoDto.Placa))
            {
                throw new ArgumentException("Já existe um veículo com essa placa.");
            }
            if (veiculoDto.MotoristaId.HasValue)
            {
                var motoristaExiste = await _context.Motoristas.AnyAsync(m => m.Id == veiculoDto.MotoristaId.Value);
                if (!motoristaExiste) throw new ArgumentException("Motorista não encontrado.");
            }
            var veiculo = _mapper.Map<Veiculo>(veiculoDto);
            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<List<ReadVeiculoDto>> GetAllVeiculosAsync(int skip, int take)
        {
            var veiculos = await _context.Veiculos
                .Include(v => v.Motorista)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            if (veiculos == null || !veiculos.Any())
            {
                // Log para verificar se há veículos retornados
                Console.WriteLine("Nenhum veículo encontrado.");
            }

            return _mapper.Map<List<ReadVeiculoDto>>(veiculos);
        }

        public async Task<ReadVeiculoDto> GetVeiculoByIdAsync(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Motorista) 
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null) return null;
            return _mapper.Map<ReadVeiculoDto>(veiculo);
        }

        public async Task<bool> UpdateVeiculoAsync(int id, UpdateVeiculoDto veiculoDto)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) return false;
            if (veiculoDto.MotoristaId.HasValue)
            {
                var motoristaExiste = await _context.Motoristas.AnyAsync(m => m.Id == veiculoDto.MotoristaId.Value);
                if (!motoristaExiste) throw new ArgumentException("Motorista não encontrado.");
            }
            _mapper.Map(veiculoDto, veiculo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVeiculoAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) return false;

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
