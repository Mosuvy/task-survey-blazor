using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserRelationDTO
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string SupervisorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}