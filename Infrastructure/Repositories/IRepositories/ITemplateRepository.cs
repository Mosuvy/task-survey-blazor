using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface ITemplateRepository
    {
        Task<List<TemplateHeader>> GetAllTemplateAsync();
        Task<TemplateHeader?> GetTemplateByIdAsync(string id);
        Task<List<TemplateHeader>?> GetTemplateByPositionIdAsync(int positionId);
        Task<TemplateHeader> CreateTemplateAsync(TemplateHeader templateHeader);
        Task<TemplateHeader?> UpdateTemplateAsync(string id, TemplateHeader templateHeader);
        Task<bool> DeleteTemplateAsync(string id);
    }
}