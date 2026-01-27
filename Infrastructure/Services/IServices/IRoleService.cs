using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.RoleDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Services.IServices
{
    public interface IRoleService
    {
        Task<List<RoleResponseDTO>> GetRoles();
        Task<RoleResponseDTO?> GetRoleById(int id);
    }
}