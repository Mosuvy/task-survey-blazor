using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.Mappers;
using TaskSurvey.Infrastructure.Repositories.IRepositories;

namespace TaskSurvey.Infrastructure.Services
{
    public class TemplateService
    {
        private readonly ITemplateRepository _repository;

        public TemplateService(ITemplateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<TemplateHeaderResponseDTO>> GetTemplateHeaders()
        {
            var templates = await _repository.GetAllTemplateAsync();
            return templates.Select(TemplateMapper.ToTemplateHeaderResponseDTO).ToList();
        }

        public async Task<TemplateHeaderResponseDTO?> GetTemplateByHeaderId(string id)
        {
            var template = await _repository.GetTemplateByIdAsync(id);
            if (template == null) return null;
            return TemplateMapper.ToTemplateHeaderResponseDTO(template);
        }

        public async Task<List<TemplateHeaderResponseDTO>?> GetTemplateByPositionId(int id)
        {
            var templates = await _repository.GetTemplateByPositionIdAsync(id);
            if (templates == null) return null;
            return templates.Select(TemplateMapper.ToTemplateHeaderResponseDTO).ToList();
        }

        public async Task<TemplateHeaderResponseDTO> CreateTemplate(TemplateHeaderRequestDTO req)
        {
            var templateEntity = TemplateMapper.ToEntity(req);
            var createdTemplate = await _repository.CreateTemplateAsync(templateEntity);
            return TemplateMapper.ToTemplateHeaderResponseDTO(createdTemplate);
        }

        public async Task<TemplateHeaderResponseDTO?> UpdateTemplate(string id, TemplateHeaderRequestDTO req)
        {
            var templateEntity = TemplateMapper.ToEntity(req);
            var updatedTemplate = await _repository.UpdateTemplateAsync(id, templateEntity);
            if (updatedTemplate == null) return null;
            return TemplateMapper.ToTemplateHeaderResponseDTO(updatedTemplate);
        }

        public async Task<bool> DeleteTemplate(string id)
        {
            return await _repository.DeleteTemplateAsync(id);
        }
    }
}