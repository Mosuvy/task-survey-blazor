using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRoleAsync();
        Task<Role?> GetRoleByIdAsync(int id);
    }
}