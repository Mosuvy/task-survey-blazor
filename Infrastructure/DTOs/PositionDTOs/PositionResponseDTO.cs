using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.PositionDTOs
{
    public class PositionResponseDTO
    {
        public int Id { get; set; }
        public required string PositionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}