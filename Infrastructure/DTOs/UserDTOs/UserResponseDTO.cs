using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.DTOs.RoleDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserResponseDTO
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public int PositionId { get; set; }
        public PositionResponseDTO? Position { get; set; }
        public required string PositionName { get; set; }
        public int RoleId { get; set; }
        public RoleResponseDTO? Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserResponseDTO? Supervisor { get; set; }
    }
}