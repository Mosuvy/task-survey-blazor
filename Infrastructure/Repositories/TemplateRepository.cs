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
    public class TemplateRepository : ITemplateRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public TemplateRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<List<TemplateHeader>> GetAllTemplateAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.TemplateHeaders
                .Include(h => h.Position)
                .Include(h => h.Items!.OrderBy(i => i.OrderNo))
                    .ThenInclude(i => i.ItemDetails)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<TemplateHeader?> GetTemplateByIdAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.TemplateHeaders
                .Include(h => h.Position)
                .Include(h => h.Items!.OrderBy(i => i.OrderNo))
                    .ThenInclude(i => i.ItemDetails)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<List<TemplateHeader>?> GetTemplateByPositionIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.TemplateHeaders
                .Include(h => h.Position)
                .Include(h => h.Items!.OrderBy(i => i.OrderNo))
                    .ThenInclude(i => i.ItemDetails)
                .Where(h => h.PositionId == id)
                .ToListAsync();
        }

        public async Task<TemplateHeader> CreateTemplateAsync(TemplateHeader templateHeader)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                templateHeader.Id = await IdGeneratorUtil.GenerateTemplateId(context);
                
                templateHeader.CreatedAt = DateTime.Now;
                templateHeader.UpdatedAt = DateTime.Now;

                foreach (var item in templateHeader.Items!)
                {
                    item.CreatedAt = DateTime.Now;
                    item.UpdatedAt = DateTime.Now;

                    foreach (var detail in item.ItemDetails!)
                    {
                        detail.CreatedAt = DateTime.Now;
                        detail.UpdatedAt = DateTime.Now;
                    }
                }

                await context.TemplateHeaders.AddAsync(templateHeader);
                await context.SaveChangesAsync();
                
                await transaction.CommitAsync();
                return templateHeader;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TemplateHeader?> UpdateTemplateAsync(string id, TemplateHeader templateHeader)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var dbHeader = await context.TemplateHeaders
                    .Include(h => h.Items!)
                        .ThenInclude(i => i.ItemDetails)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (dbHeader == null) return null;

                dbHeader.TemplateName = templateHeader.TemplateName;
                dbHeader.PositionId = templateHeader.PositionId;
                dbHeader.Theme = templateHeader.Theme;
                dbHeader.UpdatedAt = DateTime.Now;

                var incomingIds = templateHeader.Items?.Select(i => i.Id).ToList() ?? new List<int>();

                var toDelete = dbHeader.Items!.Where(di => !incomingIds.Contains(di.Id)).ToList();
                context.TemplateItems.RemoveRange(toDelete);

                foreach (var ri in templateHeader.Items ?? new List<TemplateItem>())
                {
                    var existingItem = dbHeader.Items!.FirstOrDefault(di => di.Id == ri.Id && di.Id != 0);

                    if (existingItem != null)
                    {
                        existingItem.Question = ri.Question;
                        existingItem.Type = ri.Type;
                        existingItem.OrderNo = ri.OrderNo;
                        existingItem.UpdatedAt = DateTime.Now;

                        var incomingDetailIds = ri.ItemDetails?.Select(d => d.Id).ToList() ?? new List<int>();
                        var detailsToDelete = existingItem.ItemDetails!.Where(ed => !incomingDetailIds.Contains(ed.Id)).ToList();
                        context.TemplateItemDetails.RemoveRange(detailsToDelete);

                        foreach (var rd in ri.ItemDetails ?? new List<TemplateItemDetail>())
                        {
                            var ed = existingItem.ItemDetails!.FirstOrDefault(x => x.Id == rd.Id && x.Id != 0);
                            if (ed != null) { ed.Item = rd.Item; }
                            else {
                                rd.Id = 0;
                                rd.UpdatedAt = DateTime.Now;
                                rd.CreatedAt = DateTime.Now;
                                existingItem.ItemDetails!.Add(rd);
                            }
                        }
                    }
                    else
                    {
                        ri.Id = 0; 
                        if(ri.ItemDetails != null) {
                            foreach(var d in ri.ItemDetails)
                            {
                                d.Id = 0;
                                d.UpdatedAt = DateTime.Now;
                                d.CreatedAt = DateTime.Now;   
                            }
                        }
                        dbHeader.Items!.Add(ri);
                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return dbHeader;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteTemplateAsync(string id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var header = await context.TemplateHeaders
                    .Include(h => h.Items)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (header == null) return false;

                var relatedSurveys = await context.DocumentSurveys
                    .Where(ds => ds.TemplateHeaderId == id)
                    .ToListAsync();

                foreach (var survey in relatedSurveys)
                {
                    survey.TemplateHeaderId = null;
                }

                context.TemplateHeaders.Remove(header);
                var result = await context.SaveChangesAsync() > 0;
                
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}