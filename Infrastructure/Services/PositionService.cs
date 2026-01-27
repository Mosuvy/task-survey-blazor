using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;
using TaskSurvey.Infrastructure.Mappers;
using TaskSurvey.Infrastructure.Repositories.IRepositories;

namespace TaskSurvey.Infrastructure.Services
{
    public class PositionService
    {
        private readonly IPositionRepository _repository;

        public PositionService(IPositionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<PositionResponseDTO>> GetPositions()
        {
            var positions = await _repository.GetAllPositionAsync();
            return positions.Select(PositionMapper.ToPositionResponseDTO).ToList();
        }

        public async Task<PositionResponseDTO?> GetPositionById(int id)
        {
            var position = await _repository.GetPositionByIdAsync(id);
            if(position == null) return null;
            return PositionMapper.ToPositionResponseDTO(position);
        }
    }
}