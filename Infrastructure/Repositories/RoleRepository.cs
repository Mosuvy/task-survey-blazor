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
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public RoleRepository(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Role>> GetAllRoleAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Roles.ToListAsync();
        }
        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Roles.FindAsync(id);
        }
    }
}