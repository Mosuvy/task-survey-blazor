using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Repositories.IRepositories;

namespace TaskSurvey.Infrastructure.Repositories
{
    public class UserRelationRepository : IUserRelationRepository
    {
        private readonly AppDbContext _context;

        public UserRelationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<UserRelation>> GetAllUserRelationAsync()
        {
            return await _context.UserRelations.Include(ur => ur.User).ThenInclude(u => u!.Position).Include(ur => ur.User).ThenInclude(u => u!.Role)
                .Include(ur => ur.Supervisor).ThenInclude(s => s!.Position).Include(ur => ur.Supervisor).ThenInclude(s => s!.Role)
                .ToListAsync();
        }
        
        public async Task<UserRelation?> GetUserRelationByIdAsync(int id)
        {
            return await _context.UserRelations.Include(ur => ur.User).ThenInclude(u => u!.Position).Include(ur => ur.User).ThenInclude(u => u!.Role)
                .Include(ur => ur.Supervisor).ThenInclude(s => s!.Position).Include(ur => ur.Supervisor).ThenInclude(s => s!.Role)
                .FirstOrDefaultAsync(ur => ur.Id == id);
        }

        public async Task<UserRelation?> GetUserRelationByUserIdAsync(string id)
        {
            return await _context.UserRelations.Include(ur => ur.User).ThenInclude(u => u!.Position).Include(ur => ur.User).ThenInclude(u => u!.Role)
                .Include(ur => ur.Supervisor).ThenInclude(s => s!.Position).Include(ur => ur.Supervisor).ThenInclude(s => s!.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == id);
        }

        public async Task<List<UserRelation>> GetUserRelationBySupervisorIdAsync(string id)
        {
            return await _context.UserRelations.Include(ur => ur.User).ThenInclude(u => u!.Position).Include(ur => ur.User).ThenInclude(u => u!.Role)
                .Include(ur => ur.Supervisor).ThenInclude(s => s!.Position).Include(ur => ur.Supervisor).ThenInclude(s => s!.Role)
                .Where(ur => ur.SupervisorId == id).ToListAsync();
        }
    }
}