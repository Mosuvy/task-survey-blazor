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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.Include(p => p.Position).Include(r => r.Role)
                .Include(r => r.SupervisorRelations!).ThenInclude(s => s.Supervisor).ThenInclude(s => s!.Position)
                .Include(s => s.SubordinateRelations!).ThenInclude(s => s.User).ThenInclude(s => s!.Position)
                .ToListAsync();
        }
        
        public async Task<User?> GetUserByIdAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.Include(p => p.Position).Include(r => r.Role)
                .Include(r => r.SupervisorRelations!).ThenInclude(s => s.Supervisor).ThenInclude(s => s!.Position)
                .Include(s => s.SubordinateRelations!).ThenInclude(s => s.User).ThenInclude(s => s!.Position)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<string>> GetSubordinateIdsAsync(string supervisorId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserRelations
                .Where(usr => usr.SupervisorId == supervisorId)
                .Select(usr => usr.UserId)
                .ToListAsync();
        }

        public async Task<List<User>?> GetUserByRoleIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var roleExists = await context.Roles.AnyAsync(r => r.Id == id);
            if(!roleExists) return null;

            return await context.Users.Include(p => p.Position).Include(r => r.Role)
                .Where(u => u.RoleId == id).ToListAsync();
        }

        public async Task<List<User>?> GetUserByPositionIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var positionExists = await context.Positions.AnyAsync(p => p.Id == id);
            if(!positionExists) return null;

            return await context.Users.Include(p => p.Position).Include(r => r.Role)
                .Where(u => u.PositionId == id).ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user, string supervisorId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                user.Id = await IdGeneratorUtil.GetNextFormattedUserId(context, false);
                user.PasswordHash = PasswordUtil.HashPassword("password");
                user.CreatedAt = DateTime.Now;

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var supervisor = await context.Users.AnyAsync(s => s.Id == supervisorId);
                if (supervisor)
                {
                    var relation = new UserRelation
                    {
                        UserId = user.Id,
                        SupervisorId = supervisorId,
                        CreatedAt = DateTime.Now
                    };
                    await context.UserRelations.AddAsync(relation);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                
                return (await GetUserByIdAsync(user.Id))!;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(string id, User user, string? supervisorId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (existingUser == null) return null;

                existingUser.Username = user.Username;
                if (!string.IsNullOrEmpty(user.PasswordHash)) 
                    existingUser.PasswordHash = PasswordUtil.HashPassword(user.PasswordHash);
                
                existingUser.PositionId = user.PositionId;
                existingUser.PositionName = user.PositionName;
                existingUser.RoleId = user.RoleId;
                existingUser.UpdatedAt = DateTime.Now;

                if (!string.IsNullOrEmpty(supervisorId))
                {
                    var existingRelation = await context.UserRelations.FirstOrDefaultAsync(u => u.UserId == id);
                    if (existingRelation != null)
                    {
                        existingRelation.SupervisorId = supervisorId;
                    }
                    else
                    {
                        await context.UserRelations.AddAsync(new UserRelation { 
                            UserId = id, 
                            SupervisorId = supervisorId, 
                            CreatedAt = DateTime.Now 
                        });
                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetUserByIdAsync(existingUser.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var user = await context.Users
                    .Include(u => u.SupervisorRelations)
                    .Include(u => u.SubordinateRelations)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return false;

                if (user.SupervisorRelations != null && user.SupervisorRelations.Any())
                {
                    context.UserRelations.RemoveRange(user.SupervisorRelations);
                }
                
                if (user.SubordinateRelations != null && user.SubordinateRelations.Any())
                {
                    context.UserRelations.RemoveRange(user.SubordinateRelations);
                }

                context.Users.Remove(user);
                var result = await context.SaveChangesAsync() > 0;
                
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}