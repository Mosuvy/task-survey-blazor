using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserResponseWithRelationDTO
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public UserResponseDTO? User { get; set; }
        public required string SupervisorId { get; set; }
        public UserResponseDTO? Supervisor { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}