using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;

namespace TaskSurvey.Infrastructure.Services.IServices
{
    public interface ITemplateService
    {
        Task<List<TemplateHeaderResponseDTO>> GetTemplateHeaders();
        Task<TemplateHeaderResponseDTO?> GetTemplateByHeaderId(string headerId);
        Task<List<TemplateHeaderResponseDTO>?> GetTemplateByPositionId(int positionId);
        Task<TemplateHeaderResponseDTO> CreateTemplate(TemplateHeaderRequestDTO req);
        Task<TemplateHeaderResponseDTO?> UpdateTemplate(string id, TemplateHeaderRequestDTO req);
        Task<bool> DeleteTemplate(string id);
    }
}