using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.AuthDTOs
{
    public class LoginRequestDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}