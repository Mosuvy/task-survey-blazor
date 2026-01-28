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
        private readonly AppDbContext _context;

        public TemplateRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<TemplateHeader>> GetAllTemplateAsync()
        {
            return await _context.TemplateHeaders
                .Include(h => h.Position)
                .Include(h => h.Items!.OrderBy(i => i.OrderNo))
                    .ThenInclude(i => i.ItemDetails)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<TemplateHeader?> GetTemplateByIdAsync(string id)
        {
            return await _context.TemplateHeaders
                .Include(h => h.Position)
                .Include(h => h.Items!.OrderBy(i => i.OrderNo))
                    .ThenInclude(i => i.ItemDetails)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<List<TemplateHeader>?> GetTemplateByPositionIdAsync(int id)
        {
            return await _context.TemplateHeaders
                .Include(h => h.Position)
                .Include(h => h.Items!.OrderBy(i => i.OrderNo))
                    .ThenInclude(i => i.ItemDetails)
                .Where(h => h.PositionId == id)
                .ToListAsync();
        }

        public async Task<TemplateHeader> CreateTemplateAsync(TemplateHeader templateHeader)
        {
            templateHeader.Id = await IdGeneratorUtil.GenerateTemplateId(_context);
            
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

            _context.TemplateHeaders.Add(templateHeader);
            await _context.SaveChangesAsync();
            
            return templateHeader;
        }

        public async Task<TemplateHeader?> UpdateTemplateAsync(string id, TemplateHeader templateHeader)
        {
            var dbHeader = await _context.TemplateHeaders
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
            foreach (var item in toDelete)
            {
                _context.TemplateItems.Remove(item);
            }

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
                    foreach (var d in detailsToDelete) _context.TemplateItemDetails.Remove(d);

                    foreach (var rd in ri.ItemDetails ?? new List<TemplateItemDetail>())
                    {
                        var ed = existingItem.ItemDetails!.FirstOrDefault(x => x.Id == rd.Id && x.Id != 0);
                        if (ed != null) { ed.Item = rd.Item; }
                        else {
                            rd.Id = 0;
                            existingItem.ItemDetails!.Add(rd);
                        }
                    }
                }
                else
                {
                    ri.Id = 0; 
                    if(ri.ItemDetails != null) {
                        foreach(var d in ri.ItemDetails) d.Id = 0;
                    }
                    dbHeader.Items!.Add(ri);
                }
            }

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Console.WriteLine($"DB Error: {ex.Message}");
                throw;
            }
            
            return dbHeader;
        }

        public async Task<bool> DeleteTemplateAsync(string id)
        {
            var header = await _context.TemplateHeaders
                .Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (header == null) return false;
            var relatedSurveys = await _context.DocumentSurveys
                .Where(ds => ds.TemplateHeaderId == id)
                .ToListAsync();

            foreach (var survey in relatedSurveys)
            {
                survey.TemplateHeaderId = null;
            }
            _context.TemplateHeaders.Remove(header);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}