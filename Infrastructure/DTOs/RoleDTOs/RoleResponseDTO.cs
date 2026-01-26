using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.RoleDTOs
{
    public class RoleResponseDTO
    {
        public int Id { get; set; }
        public required string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}