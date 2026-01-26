using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.RoleDTOs;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserResponseDTO
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public int PositionId { get; set; }
        public string PositionLevel { get; set; } = string.Empty;
        public required string PositionName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}