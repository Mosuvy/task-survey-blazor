using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface IPositionRepository
    {
        Task<List<Position>> GetAllPositionAsync();
        Task<Position?> GetPositionByIdAsync(int id);
    }
}