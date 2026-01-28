using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Repositories.IRepositories;
using TaskSurvey.Infrastructure.Utils;

namespace TaskSurvey.Infrastructure.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<DocumentSurvey>> GetAllDocumentSurveyAsync()
        {
            return await _context.DocumentSurveys
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .Include(ds => ds.SurveyItems)!
                    .ThenInclude(si => si.CheckBox)
                .OrderByDescending(ds => ds.CreatedAt)
                .ToListAsync();
        }

        public async Task<DocumentSurvey?> GetDocumentSurveyByIdAsync(string id)
        {
            return await _context.DocumentSurveys
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .Include(ds => ds.SurveyItems!.OrderBy(i => i.OrderNo))
                    .ThenInclude(si => si.CheckBox)
                .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        public async Task<List<DocumentSurvey>?> GetDocumentSurveyByUserIdAsync(string id)
        {
            return await _context.DocumentSurveys
                .Where(ds => ds.RequesterId == id)
                .Include(ds => ds.Header)
                .OrderByDescending(ds => ds.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<DocumentSurvey>?> GetDocumentSurveyByStatusAsync(string status)
        {
            if (!Enum.TryParse<StatusType>(status, true, out var statusEnum)) return null;

            return await _context.DocumentSurveys
                .Where(ds => ds.Status == statusEnum)
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .ToListAsync();
        }

        public async Task<DocumentSurvey> CreateDocumentSurveyAsync(DocumentSurvey documentSurvey)
        {
            documentSurvey.Id = await IdGeneratorUtil.GenerateSurveyId(_context);
            
            documentSurvey.CreatedAt = DateTime.Now;
            documentSurvey.UpdatedAt = DateTime.Now;

            if (documentSurvey.SurveyItems != null)
            {
                foreach (var item in documentSurvey.SurveyItems)
                {
                    item.DocumentSurveyId = documentSurvey.Id; 
                    item.CreatedAt = DateTime.Now;
                    item.UpdatedAt = DateTime.Now;

                    if (item.CheckBox != null)
                    {
                        foreach (var detail in item.CheckBox)
                        {
                            detail.CreatedAt = DateTime.Now;
                            detail.UpdatedAt = DateTime.Now;
                        }
                    }
                }
            }

            await _context.DocumentSurveys.AddAsync(documentSurvey);
            await _context.SaveChangesAsync();
            return documentSurvey;
        }

        public async Task<DocumentSurvey?> UpdateDocumentSurveyAsync(string id, DocumentSurvey documentSurvey)
        {
            var dbSurvey = await _context.DocumentSurveys
                .Include(ds => ds.SurveyItems!)
                    .ThenInclude(si => si.CheckBox)
                .FirstOrDefaultAsync(ds => ds.Id == id);

            if (dbSurvey == null) return null;

            dbSurvey.Status = documentSurvey.Status;
            dbSurvey.UpdatedAt = DateTime.Now;

            var incomingItemIds = documentSurvey.SurveyItems?.Select(i => i.Id).Where(x => x > 0).ToList() ?? new List<int>();
            
            var toDeleteItems = dbSurvey.SurveyItems!.Where(i => !incomingItemIds.Contains(i.Id)).ToList();
            _context.DocumentSurveyItems.RemoveRange(toDeleteItems);

            foreach (var ri in documentSurvey.SurveyItems ?? new List<DocumentSurveyItem>())
            {
                var ei = dbSurvey.SurveyItems!.FirstOrDefault(i => i.Id == ri.Id && i.Id > 0);
                if (ei != null)
                {
                    ei.Answer = ri.Answer;
                    ei.UpdatedAt = DateTime.Now;

                    var incomingDetailIds = ri.CheckBox?.Select(d => d.Id).Where(x => x > 0).ToList() ?? new List<int>();
                    var toDeleteDetails = ei.CheckBox!.Where(d => !incomingDetailIds.Contains(d.Id)).ToList();
                    _context.DocumentItemDetails.RemoveRange(toDeleteDetails);

                    foreach (var rd in ri.CheckBox ?? new List<DocumentItemDetail>())
                    {
                        var ed = ei.CheckBox!.FirstOrDefault(d => d.Id == rd.Id && d.Id > 0);
                        if (ed != null)
                        {
                            ed.IsChecked = rd.IsChecked;
                            ed.UpdatedAt = DateTime.Now;
                        }
                        else
                        {
                            rd.Id = 0;
                            rd.CreatedAt = DateTime.Now;
                            rd.UpdatedAt = DateTime.Now;
                            ei.CheckBox!.Add(rd);
                        }
                    }
                }
                else
                {
                    ri.Id = 0;
                    ri.DocumentSurveyId = dbSurvey.Id;
                    ri.CreatedAt = DateTime.Now;
                    ri.UpdatedAt = DateTime.Now;
                    dbSurvey.SurveyItems!.Add(ri);
                }
            }

            await _context.SaveChangesAsync();
            return dbSurvey;
        }

        public async Task<DocumentSurvey?> UpdateDocumentSurveyStatusAsync(string id, string status)
        {
            var survey = await _context.DocumentSurveys.FindAsync(id);
            if (survey == null || !Enum.TryParse<StatusType>(status, true, out var statusEnum)) 
                return null;

            survey.Status = statusEnum;
            survey.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return survey;
        }

        public async Task<bool> DeleteDocumentSurveyAsync(string id)
        {
            var survey = await _context.DocumentSurveys.FindAsync(id);
            if (survey == null) return false;

            _context.DocumentSurveys.Remove(survey);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}