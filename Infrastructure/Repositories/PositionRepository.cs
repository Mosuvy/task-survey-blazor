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
        private readonly AppDbContext _context;

        public PositionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Position>> GetAllPositionAsync()
        {
            return await _context.Positions.ToListAsync();
        }
        public async Task<Position?> GetPositionByIdAsync(int id)
        {
            return await _context.Positions.FindAsync(id);
        }
    }
}