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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SurveyRepository(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<DocumentSurvey>> GetAllDocumentSurveyAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DocumentSurveys
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .Include(ds => ds.SurveyItems)!
                    .ThenInclude(si => si.CheckBox)
                .OrderByDescending(ds => ds.CreatedAt)
                .ToListAsync();
        }

        public async Task<DocumentSurvey?> GetDocumentSurveyByIdAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DocumentSurveys
                .AsNoTracking()
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .Include(ds => ds.SurveyItems!.OrderBy(i => i.OrderNo))
                    .ThenInclude(si => si.CheckBox)
                .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        public async Task<List<DocumentSurvey>?> GetDocumentSurveyByUserIdAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DocumentSurveys
                .Where(ds => ds.RequesterId == id)
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .OrderByDescending(ds => ds.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<DocumentSurvey>?> GetDocumentSurveyByStatusAsync(string status)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (!Enum.TryParse<StatusType>(status, true, out var statusEnum)) return null;

            return await context.DocumentSurveys
                .Where(ds => ds.Status == statusEnum)
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .ToListAsync();
        }

        public async Task<List<DocumentSurvey>?> GetDocumentSurveyForSupervisorAsync(string id, string status)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (!Enum.TryParse<StatusType>(status, true, out var statusEnum)) return null;

            return await context.DocumentSurveys
                .AsNoTracking()
                .Include(ds => ds.Requester)
                .Include(ds => ds.Header)
                .Where(ds => ds.RequesterId == id && 
                            (ds.Status == statusEnum || ds.Status == StatusType.Confirmed || ds.Status == StatusType.Rejected))
                .ToListAsync();
        }

        public async Task<DocumentSurvey> CreateDocumentSurveyAsync(DocumentSurvey documentSurvey)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                documentSurvey.Id = await IdGeneratorUtil.GenerateSurveyId(context);
                
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

                await context.DocumentSurveys.AddAsync(documentSurvey);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return documentSurvey;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DocumentSurvey?> UpdateDocumentSurveyAsync(string id, DocumentSurvey documentSurvey)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var dbSurvey = await context.DocumentSurveys
                    .Include(ds => ds.SurveyItems!)
                        .ThenInclude(si => si.CheckBox)
                    .FirstOrDefaultAsync(ds => ds.Id == id);

                if (dbSurvey == null) return null;

                dbSurvey.Status = documentSurvey.Status;
                dbSurvey.UpdatedAt = DateTime.Now;

                var incomingItemIds = documentSurvey.SurveyItems?.Select(i => i.Id).Where(x => x > 0).ToList() ?? new List<int>();
                var toDeleteItems = dbSurvey.SurveyItems!.Where(i => !incomingItemIds.Contains(i.Id)).ToList();
                context.DocumentSurveyItems.RemoveRange(toDeleteItems);

                foreach (var ri in documentSurvey.SurveyItems ?? new List<DocumentSurveyItem>())
                {
                    var ei = dbSurvey.SurveyItems!.FirstOrDefault(i => i.Id == ri.Id && i.Id > 0);
                    if (ei != null)
                    {
                        ei.Answer = ri.Answer;
                        ei.UpdatedAt = DateTime.Now;

                        var incomingDetailIds = ri.CheckBox?.Select(d => d.Id).Where(x => x > 0).ToList() ?? new List<int>();
                        var toDeleteDetails = ei.CheckBox!.Where(d => !incomingDetailIds.Contains(d.Id)).ToList();
                        context.DocumentItemDetails.RemoveRange(toDeleteDetails);

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

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return dbSurvey;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DocumentSurvey?> UpdateDocumentSurveyFromLatestTemplate(string id, DocumentSurvey documentSurvey)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var dbSurvey = await context.DocumentSurveys
                    .Include(ds => ds.SurveyItems)!
                    .ThenInclude(si => si.CheckBox)
                    .FirstOrDefaultAsync(ds => ds.Id == id);

                if (dbSurvey == null) return null;

                var oldItems = dbSurvey.SurveyItems?.ToList() ?? new List<DocumentSurveyItem>();
                dbSurvey.UpdatedAtTemplate = documentSurvey.UpdatedAtTemplate;
                dbSurvey.UpdatedAt = DateTime.Now;
                dbSurvey.TemplateHeaderId = documentSurvey.TemplateHeaderId;

                if (dbSurvey.SurveyItems != null && dbSurvey.SurveyItems.Any())
                {
                    context.DocumentSurveyItems.RemoveRange(dbSurvey.SurveyItems);
                    await context.SaveChangesAsync();
                }

                var newItemsToInsert = new List<DocumentSurveyItem>();
                foreach (var templateItem in documentSurvey.SurveyItems ?? new List<DocumentSurveyItem>())
                {
                    var match = oldItems.FirstOrDefault(o => o.Question == templateItem.Question && o.Type == templateItem.Type);
                    var newItem = new DocumentSurveyItem
                    {
                        DocumentSurveyId = id,
                        Question = templateItem.Question,
                        Type = templateItem.Type,
                        OrderNo = templateItem.OrderNo,
                        TemplateItemId = templateItem.TemplateItemId,
                        Answer = match?.Answer ?? "",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CheckBox = new List<DocumentItemDetail>()
                    };

                    if (templateItem.Type == QuestionType.CheckBox)
                    {
                        foreach (var templateDetail in templateItem.CheckBox ?? new List<DocumentItemDetail>())
                        {
                            var detailMatch = match?.CheckBox?.FirstOrDefault(c => c.Item == templateDetail.Item);
                            newItem.CheckBox.Add(new DocumentItemDetail
                            {
                                Item = templateDetail.Item,
                                TemplateItemDetailId = templateDetail.TemplateItemDetailId,
                                IsChecked = detailMatch?.IsChecked ?? false,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            });
                        }
                    }
                    newItemsToInsert.Add(newItem);
                }

                dbSurvey.SurveyItems = newItemsToInsert;
                context.DocumentSurveys.Update(dbSurvey);
                
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return dbSurvey;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteDocumentSurveyAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var survey = await context.DocumentSurveys.FindAsync(id);
                if (survey == null) return false;

                context.DocumentSurveys.Remove(survey);
                var result = await context.SaveChangesAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        
        public async Task<DocumentSurvey?> UpdateDocumentSurveyStatusAsync(string id, string status)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var survey = await context.DocumentSurveys.FindAsync(id);
            if (survey == null || !Enum.TryParse<StatusType>(status, true, out var statusEnum)) 
                return null;

            survey.Status = statusEnum;
            survey.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return survey;
        }
    }
}