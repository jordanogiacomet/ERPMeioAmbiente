using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Services
{
    public interface IFuncionarioService
    {
        Task<Funcionario> AddFuncionarioAsync(CreateFuncionarioDto funcionarioDto);
        Task<List<ReadFuncionarioDto>> GetAllFuncionariosAsync(int skip, int take);
        Task<ReadFuncionarioDto> GetFuncionarioByIdAsync(int id);
        Task<bool> UpdateFuncionarioAsync(int id, UpdateFuncionarioDto funcionarioDto);
        Task<bool> DeleteFuncionarioAsync(int id);
    }

    public class FuncionarioService : IFuncionarioService
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public FuncionarioService(ERPMeioAmbienteContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Funcionario> AddFuncionarioAsync(CreateFuncionarioDto funcionarioDto)
        {
            var identityUser = new IdentityUser
            {
                Email = funcionarioDto.Email,
                UserName = funcionarioDto.Email
            };

            var result = await _userManager.CreateAsync(identityUser, funcionarioDto.Password);
            if (!result.Succeeded)
            {
                throw new System.Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(identityUser, "Funcionario");

            Funcionario funcionario = _mapper.Map<Funcionario>(funcionarioDto);
            funcionario.UserId = identityUser.Id;

            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            return funcionario;
        }

        public async Task<List<ReadFuncionarioDto>> GetAllFuncionariosAsync(int skip, int take)
        {
            var funcionarios = await _context.Funcionarios.Skip(skip).Take(take).ToListAsync();
            return _mapper.Map<List<ReadFuncionarioDto>>(funcionarios);
        }

        public async Task<ReadFuncionarioDto> GetFuncionarioByIdAsync(int id)
        {
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(funcionario => funcionario.Id == id);
            if (funcionario == null)
            {
                return null;
            }
            return _mapper.Map<ReadFuncionarioDto>(funcionario);
        }

        public async Task<bool> UpdateFuncionarioAsync(int id, UpdateFuncionarioDto funcionarioDto)
        {
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(funcionario => funcionario.Id == id);
            if (funcionario == null) return false;

            _mapper.Map(funcionarioDto, funcionario);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Erro ao atualizar o funcionário com ID {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteFuncionarioAsync(int id)
        {
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(funcionario => funcionario.Id == id);
            if (funcionario == null) return false;

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}