using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<List<User>?> GetUserByRoleIdAsync(int id);
        Task<List<User>?> GetUserByPositionIdAsync(int id);
        Task<User> CreateUserAsync(User user, string supervisor);
        Task<User?> UpdateUserAsync(string id, User user, string? supervisor);
        Task<bool> DeleteUserAsync(string id);
    }
}