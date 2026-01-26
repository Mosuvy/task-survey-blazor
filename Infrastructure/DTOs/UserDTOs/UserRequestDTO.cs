using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserRequestDTO
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public int PositionId { get; set; }
        public required string PositionName { get; set; }
        public int RoleId { get; set; }
        public string? SupervisorId { get; set; }
    }
}