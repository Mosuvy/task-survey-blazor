using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.AuthDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface IAuthRepository
    {
        Task<User?> LoginAsync(LoginRequestDTO req); 
    }
}