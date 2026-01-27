using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserRequestDTO
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        public int PositionId { get; set; }
        [Required]
        public string PositionName { get; set; } = null!;
        public int RoleId { get; set; }
        public string? SupervisorId { get; set; }
    }
}