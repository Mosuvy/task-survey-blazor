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
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> LoginAsync(LoginRequestDTO req)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if(userEntity == null) return null!;

            var isRoleExist = await _context.Roles.FindAsync(userEntity.RoleId);
            if(isRoleExist == null) return null!;
            
            var isPasswordValid = PasswordUtil.VerifyPassword(userEntity.PasswordHash, req.Password);
            Console.WriteLine(isPasswordValid);
            
            if(!isPasswordValid) return null!;

            return userEntity;
        }
    }
}