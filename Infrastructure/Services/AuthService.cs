using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using TaskSurvey.Infrastructure.DTOs.AuthDTOs;
using TaskSurvey.Infrastructure.Mappers;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Services.IServices;

namespace TaskSurvey.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;

        public AuthService(IAuthRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<User?> Login(LoginRequestDTO req)
        {
            return await _repository.LoginAsync(req);
        }
    }
}