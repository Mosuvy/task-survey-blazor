using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Utils;

namespace TaskSurvey.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(p => p.Position).Include(r => r.Role)
                .Include(r => r.SupervisorRelations!).ThenInclude(s => s.Supervisor).ThenInclude(s => s!.Position)
                .ToListAsync();
        }
        
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users.Include(p => p.Position).Include(r => r.Role)
                .Include(r => r.SupervisorRelations!).ThenInclude(s => s.Supervisor).ThenInclude(s => s!.Position)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>?> GetUserByRoleIdAsync(int id)
        {
            var isRole = await _context.Roles.FindAsync(id);
            if(isRole == null) return null;
            return await _context.Users.Include(p => p.Position).Include(r => r.Role).Where(u => u.RoleId == id).ToListAsync();
        }

        public async Task<List<User>?> GetUserByPositionIdAsync(int id)
        {
            var isPosition = await _context.Positions.FindAsync(id);
            if(isPosition == null) return null;
            return await _context.Users.Include(p => p.Position).Include(r => r.Role).Where(u => u.PositionId == id).ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user, string supervisor)
        {
            user.Id = await IdGeneratorUtil.GetNextFormattedUserId(_context);
            // user.PasswordHash = PasswordUtil.HashPassword(user.PasswordHash);
            user.PasswordHash = PasswordUtil.HashPassword("password");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var isSupervisor = await _context.Users.FirstOrDefaultAsync(s => s.Id == supervisor);
            if(isSupervisor == null) return null!;

            var relation = new UserRelation
            {
                UserId = user.Id,
                SupervisorId = supervisor,
                CreatedAt = DateTime.Now
            };
            _context.UserRelations.Add(relation);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUserAsync(string id, User user, string? supervisor)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(existingUser == null) return null;

            existingUser.Username = user.Username;
            if(!string.IsNullOrEmpty(user.PasswordHash)) 
                existingUser.PasswordHash = PasswordUtil.HashPassword(user.PasswordHash);
            existingUser.PositionId = user.PositionId;
            existingUser.PositionName = user.PositionName;
            existingUser.RoleId = user.RoleId;
            existingUser.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(supervisor))
            {
                var existingRelation = await _context.UserRelations.FirstOrDefaultAsync(u => u.UserId == id);
                if(existingRelation != null) existingRelation.SupervisorId = supervisor;
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var isUserExist = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(isUserExist == null) return false;

            _context.Users.Remove(isUserExist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}