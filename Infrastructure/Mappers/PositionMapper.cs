using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Mappers
{
    public class PositionMapper
    {
        public static PositionResponseDTO ToPositionResponseDTO(Position position)
        {
            return new PositionResponseDTO
            {
                Id = position.Id,
                PositionLevel = position.PositionLevel,
                CreatedAt = position.CreatedAt  
            };
        }
    }
}