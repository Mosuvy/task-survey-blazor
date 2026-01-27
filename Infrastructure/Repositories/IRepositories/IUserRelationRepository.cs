using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface IUserRelationRepository
    {
        Task<List<UserRelation>> GetAllUserRelationAsync();
        Task<UserRelation?> GetUserRelationByIdAsync(int id);
        Task<UserRelation?> GetUserRelationByUserIdAsync(string id);
        Task<List<UserRelation>> GetUserRelationBySupervisorIdAsync(string id);
    }
}