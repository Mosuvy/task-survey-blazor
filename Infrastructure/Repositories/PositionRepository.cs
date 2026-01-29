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
    public class PositionRepository : IPositionRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PositionRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<List<Position>> GetAllPositionAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Positions.ToListAsync();
        }
        public async Task<Position?> GetPositionByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Positions.FindAsync(id);
        }
    }
}