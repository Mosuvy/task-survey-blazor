using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.RoleDTOs;
using TaskSurvey.Infrastructure.Mappers;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Services.IServices;

namespace TaskSurvey.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<RoleResponseDTO>> GetRoles()
        {
            var roles = await _repository.GetAllRoleAsync();
            return roles.Select(RoleMapper.ToRoleResponseDTO).ToList();
        }

        public async Task<RoleResponseDTO?> GetRoleById(int id)
        {
            var role = await _repository.GetRoleByIdAsync(id);
            if(role == null) return null;
            return RoleMapper.ToRoleResponseDTO(role);
        }
    }
}