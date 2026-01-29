using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.DTOs.AuthDTOs;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Utils;

namespace TaskSurvey.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AuthRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<User?> LoginAsync(LoginRequestDTO req)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var userEntity = await context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.Username == req.Username);
            if(userEntity == null) return null!;

            var isRoleExist = await context.Roles.FindAsync(userEntity.RoleId);
            if(isRoleExist == null) return null!;
            
            var isPasswordValid = PasswordUtil.VerifyPassword(userEntity.PasswordHash, req.Password);
            Console.WriteLine(isPasswordValid);
            
            if(!isPasswordValid) return null!;

            return userEntity;
        }
    }
}