using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.Repositories.IRepositories
{
    public interface ISurveyRepository
    {
        Task<List<DocumentSurvey>> GetAllDocumentSurveyAsync();
        Task<DocumentSurvey?> GetDocumentSurveyByIdAsync(string id);
        Task<List<DocumentSurvey>?> GetDocumentSurveyByUserIdAsync(string id);
        Task<List<DocumentSurvey>?> GetDocumentSurveyByStatusAsync(string status);
        Task<DocumentSurvey> CreateDocumentSurveyAsync(DocumentSurvey documentSurvey);
        Task<DocumentSurvey?> UpdateDocumentSurveyAsync(string id, DocumentSurvey documentSurvey);
        Task<DocumentSurvey?> UpdateDocumentSurveyStatusAsync(string id, string status);
        Task<bool> DeleteDocumentSurveyAsync(string id);
    }
}