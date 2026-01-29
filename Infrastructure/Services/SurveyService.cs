using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;
using TaskSurvey.Infrastructure.Mappers;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Services.IServices;

namespace TaskSurvey.Infrastructure.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _repository;
        private readonly ITemplateRepository _templaterepository;

        public SurveyService(ISurveyRepository repository, ITemplateRepository templaterepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _templaterepository = templaterepository ?? throw new ArgumentNullException(nameof(templaterepository));
        }

        public async Task<List<SurveyHeaderResponseDTO>> GetSurveyHeaders()
        {
            var data = await _repository.GetAllDocumentSurveyAsync();
            return data.Select(SurveyMapper.ToSurveyResponseDTO).ToList();
        }

        public async Task<SurveyHeaderResponseDTO?> GetSurveyHeaderById(string id)
        {
            var data = await _repository.GetDocumentSurveyByIdAsync(id);
            return data != null ? SurveyMapper.ToSurveyResponseDTO(data) : null;
        }

        public async Task<List<SurveyHeaderResponseDTO>?> GetSurveyHeaderByUserId(string id)
        {
            var data = await _repository.GetDocumentSurveyByUserIdAsync(id);
            return data?.Select(SurveyMapper.ToSurveyResponseDTO!).ToList();
        }

        public async Task<List<SurveyHeaderResponseDTO>?> GetSurveyHeaderByStatus(string status)
        {
            var data = await _repository.GetDocumentSurveyByStatusAsync(status);
            return data?.Select(SurveyMapper.ToSurveyResponseDTO).ToList();
        }

        public async Task<List<SurveyHeaderResponseDTO>?> GetDocumentSurveyForSupervisor(string id, string status)
        {
            var data = await _repository.GetDocumentSurveyForSupervisorAsync(id, status);
            return data?.Select(SurveyMapper.ToSurveyResponseDTO).ToList();
        }

        public async Task<SurveyHeaderResponseDTO> CreateSurveyHeader(SurveyHeaderRequestDTO surveyHeaderRequestDTO)
        {
            var entity = SurveyMapper.ToEntity(surveyHeaderRequestDTO);
            
            var result = await _repository.CreateDocumentSurveyAsync(entity);
            
            return SurveyMapper.ToSurveyResponseDTO(result);
        }

        public async Task<SurveyHeaderResponseDTO?> UpdateSurveyHeader(string id, SurveyHeaderRequestDTO surveyHeaderRequestDTO)
        {
            var entity = SurveyMapper.ToEntity(surveyHeaderRequestDTO);
            var result = await _repository.UpdateDocumentSurveyAsync(id, entity);
            
            return result != null ? SurveyMapper.ToSurveyResponseDTO(result) : null;
        }

        public async Task<SurveyHeaderResponseDTO?> UpdateSurveyHeaderStatus(string id, string status)
        {
            var result = await _repository.UpdateDocumentSurveyStatusAsync(id, status);
            return result != null ? SurveyMapper.ToSurveyResponseDTO(result) : null;
        }

        public async Task<SurveyHeaderResponseDTO?> UpdateSurveyHeaderFromLatestTemplate(string id, SurveyHeaderRequestDTO request)
        {
            var latestTemplate = await _templaterepository.GetTemplateByIdAsync(request.TemplateHeaderId!);
            if (latestTemplate == null) return null;

            var blueprintEntity = SurveyMapper.ToEntityFromTemplate(latestTemplate, id, request.RequesterId, request.Status);

            var result = await _repository.UpdateDocumentSurveyFromLatestTemplate(id, blueprintEntity);
            
            return result != null ? SurveyMapper.ToSurveyResponseDTO(result) : null;
        }

        public async Task<bool> DeleteSurveyHeader(string id)
        {
            return await _repository.DeleteDocumentSurveyAsync(id);
        }
    }
}