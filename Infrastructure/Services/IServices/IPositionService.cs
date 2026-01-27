using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Services.IServices
{
    public interface IPositionService
    {
        Task<List<PositionResponseDTO>> GetPositions();
        Task<PositionResponseDTO?> GetPositionById(int id);
    }
}