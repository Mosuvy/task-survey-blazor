using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.UserDTOs
{
    public class UserRequestWithRelationDTO
    {
        public required string UserId { get; set; }
        public required string SupervisorId { get; set; }
    }
}