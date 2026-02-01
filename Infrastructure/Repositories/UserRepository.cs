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

                if (!string.IsNullOrWhiteSpace(supervisorId))
                {
                    var supervisor = await context.Users.AnyAsync(s => s.Id == supervisorId);
                    if (supervisor)
                    {
                        var wouldCreateCircular = await SupervisorValidationUtil.WouldCreateCircularReference(
                            context, user.Id, supervisorId);
                        
                        if (wouldCreateCircular)
                        {
                            throw new InvalidOperationException(
                                "Cannot set this supervisor: it would create a circular reference in the hierarchy.");
                        }

                        var relation = new UserRelation
                        {
                            UserId = user.Id,
                            SupervisorId = supervisorId,
                            CreatedAt = DateTime.Now
                        };
                        await context.UserRelations.AddAsync(relation);
                        await context.SaveChangesAsync();

                        await UpdateSupervisorRoleIfNeeded(context, supervisorId);
                    }
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

        /// <summary>
        /// Update user role to Overseer if they have subordinates and are currently a User
        /// </summary>
        private async Task UpdateSupervisorRoleIfNeeded(AppDbContext context, string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            if (user.RoleId == 2)
            {
                var hasSubordinates = await context.UserRelations.AnyAsync(ur => ur.SupervisorId == userId);
                
                if (hasSubordinates)
                {
                    user.RoleId = 3;
                    user.UpdatedAt = DateTime.Now;
                    await context.SaveChangesAsync();
                }
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

                var oldSupervisorId = await context.UserRelations
                    .Where(ur => ur.UserId == id)
                    .Select(ur => ur.SupervisorId)
                    .FirstOrDefaultAsync();

                existingUser.Username = user.Username;
                if (!string.IsNullOrEmpty(user.PasswordHash)) 
                    existingUser.PasswordHash = PasswordUtil.HashPassword(user.PasswordHash);
                
                existingUser.PositionId = user.PositionId;
                existingUser.PositionName = user.PositionName;
                existingUser.UpdatedAt = DateTime.Now;

                var existingRelation = await context.UserRelations.FirstOrDefaultAsync(u => u.UserId == id);

                if (!string.IsNullOrWhiteSpace(supervisorId))
                {
                    var wouldCreateCircular = await SupervisorValidationUtil.WouldCreateCircularReference(
                        context, id, supervisorId);
                    
                    if (wouldCreateCircular)
                    {
                        throw new InvalidOperationException(
                            "Cannot set this supervisor: it would create a circular reference in the hierarchy.");
                    }

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

                    var hasSubordinates = await context.UserRelations.AnyAsync(ur => ur.SupervisorId == id);
                    if (hasSubordinates)
                    {
                        existingUser.RoleId = 3; // Overseer
                    }
                    else
                    {
                        existingUser.RoleId = 2; // User
                    }
                }
                else
                {
                    // Supervisor
                    if (existingRelation != null)
                    {
                        context.UserRelations.Remove(existingRelation);
                    }
                    existingUser.RoleId = 1;
                }

                await context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(supervisorId))
                {
                    await UpdateUserRoleBasedOnSubordinates(context, supervisorId);
                }

                if (!string.IsNullOrWhiteSpace(oldSupervisorId) && oldSupervisorId != supervisorId)
                {
                    await UpdateUserRoleBasedOnSubordinates(context, oldSupervisorId);
                }

                await transaction.CommitAsync();
                return await GetUserByIdAsync(existingUser.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Update user role based on their current subordinates and supervisor status
        /// </summary>
        private async Task UpdateUserRoleBasedOnSubordinates(AppDbContext context, string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            var hasSupervisor = await context.UserRelations.AnyAsync(ur => ur.UserId == userId);
            
            var hasSubordinates = await context.UserRelations.AnyAsync(ur => ur.SupervisorId == userId);

            int newRoleId;
            
            if (!hasSupervisor && hasSubordinates)
            {
                newRoleId = 1; // Supervisor
            }
            else if (hasSupervisor && hasSubordinates)
            {
                newRoleId = 3; // Overseer
            }
            else if (hasSupervisor && !hasSubordinates)
            {
                newRoleId = 2; // User
            }
            else
            {
                newRoleId = 1; // Supervisor
            }

            if (user.RoleId != newRoleId)
            {
                user.RoleId = newRoleId;
                user.UpdatedAt = DateTime.Now;
                await context.SaveChangesAsync();
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

                string? supervisorId = user.SupervisorRelations?.FirstOrDefault()?.SupervisorId;
                
                List<string> subordinateIds = user.SubordinateRelations?
                    .Select(sr => sr.UserId)
                    .ToList() ?? new List<string>();

                if (user.SupervisorRelations != null && user.SupervisorRelations.Any())
                {
                    context.UserRelations.RemoveRange(user.SupervisorRelations);
                }
                
                if (user.SubordinateRelations != null && user.SubordinateRelations.Any())
                {
                    context.UserRelations.RemoveRange(user.SubordinateRelations);
                }

                context.Users.Remove(user);
                
                await context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(supervisorId))
                {
                    await UpdateUserRoleBasedOnSubordinates(context, supervisorId);
                }

                foreach (var subordinateId in subordinateIds)
                {
                    await UpdateUserRoleBasedOnSubordinates(context, subordinateId);
                }
                
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}