using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;

namespace TaskSurvey.Infrastructure.Services.IServices
{
    public interface ISurveyService
    {
        Task<List<SurveyHeaderResponseDTO>> GetSurveyHeaders();
        Task<SurveyHeaderResponseDTO?> GetSurveyHeaderById(string id);
        Task<List<SurveyHeaderResponseDTO>?> GetSurveyHeaderByUserId(string id);
        Task<List<SurveyHeaderResponseDTO>?> GetSurveyHeaderByStatus(string status);
        Task<List<SurveyHeaderResponseDTO>?> GetDocumentSurveyForSupervisor(string id, string status);
        Task<SurveyHeaderResponseDTO> CreateSurveyHeader(SurveyHeaderRequestDTO surveyHeaderRequestDTO);
        Task<SurveyHeaderResponseDTO?> UpdateSurveyHeader(string id, SurveyHeaderRequestDTO surveyHeaderRequestDTO);
        Task<SurveyHeaderResponseDTO?> UpdateSurveyHeaderStatus(string id, string status);
        Task<SurveyHeaderResponseDTO?> UpdateSurveyHeaderFromLatestTemplate(string id, SurveyHeaderRequestDTO surveyHeaderRequestDTO);
        Task<bool> DeleteSurveyHeader(string id);
    }
}