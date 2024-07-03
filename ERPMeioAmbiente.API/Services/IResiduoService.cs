using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Services
{
    public interface IResiduoService
    {
        Task<Residuo> AddResiduoAsync(CreateResiduoDto residuoDto);
        Task<List<ReadResiduoDto>> GetAllResiduosAsync(int skip, int take);
        Task<ReadResiduoDto> GetResiduoByIdAsync(int id);
        Task<bool> UpdateResiduoAsync(int id, UpdateResiduoDto residuoDto);
        Task<bool> DeleteResiduoAsync(int id);
    }

    public class ResiduoService : IResiduoService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public ResiduoService(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Residuo> AddResiduoAsync(CreateResiduoDto residuoDto)
        {
            Residuo residuo = _mapper.Map<Residuo>(residuoDto);
            _context.Residuos.Add(residuo);
            await _context.SaveChangesAsync();
            return residuo;
        }

        public async Task<List<ReadResiduoDto>> GetAllResiduosAsync(int skip, int take)
        {
            var residuos = await _context.Residuos.Skip(skip).Take(take).ToListAsync();
            return _mapper.Map<List<ReadResiduoDto>>(residuos);
        }

        public async Task<ReadResiduoDto> GetResiduoByIdAsync(int id)
        {
            var residuo = await _context.Residuos.FirstOrDefaultAsync(residuo => residuo.Id == id);
            if (residuo == null)
            {
                return null;
            }
            return _mapper.Map<ReadResiduoDto>(residuo);
        }

        public async Task<bool> UpdateResiduoAsync(int id, UpdateResiduoDto residuoDto)
        {
            var residuo = await _context.Residuos.FirstOrDefaultAsync(residuo => residuo.Id == id);
            if (residuo == null) return false;
            _mapper.Map(residuoDto, residuo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteResiduoAsync(int id)
        {
            var residuo = await _context.Residuos.FirstOrDefaultAsync(residuo => residuo.Id == id);
            if (residuo == null) return false;
            _context.Residuos.Remove(residuo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}