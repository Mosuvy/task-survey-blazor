using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using TaskSurvey.Infrastructure.DTOs.AuthDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Services.IServices
{
    public interface IAuthService
    {
        Task<User?> Login(LoginRequestDTO req);
    }
}