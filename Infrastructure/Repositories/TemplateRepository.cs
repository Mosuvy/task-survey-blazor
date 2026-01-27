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

            _context.TemplateHeaders.Add(templateHeader);
            await _context.SaveChangesAsync();
            
            return templateHeader;
        }

        public async Task<TemplateHeader?> UpdateTemplateAsync(string id, TemplateHeader templateHeader)
        {
            var existingHeader = await _context.TemplateHeaders
                .Include(h => h.Items)!
                    .ThenInclude(i => i.ItemDetails)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (existingHeader == null) return null;

            _context.Entry(existingHeader).CurrentValues.SetValues(templateHeader);
            existingHeader.UpdatedAt = DateTime.Now;

            foreach (var existingItem in existingHeader.Items!.ToList())
            {
                if (!templateHeader.Items!.Any(i => i.Id == existingItem.Id))
                    _context.TemplateItems.Remove(existingItem);
            }

            foreach (var requestItem in templateHeader.Items!)
            {
                var existingItem = existingHeader.Items!.FirstOrDefault(i => i.Id == requestItem.Id && i.Id != 0);

                if (existingItem != null)
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(requestItem);
                    
                    foreach (var existingDetail in existingItem.ItemDetails!.ToList())
                    {
                        if (!requestItem.ItemDetails!.Any(d => d.Id == existingDetail.Id))
                            _context.TemplateItemDetails.Remove(existingDetail);
                    }

                    foreach (var requestDetail in requestItem.ItemDetails!)
                    {
                        var existingDetail = existingItem.ItemDetails!.FirstOrDefault(d => d.Id == requestDetail.Id && d.Id != 0);
                        if (existingDetail != null)
                            _context.Entry(existingDetail).CurrentValues.SetValues(requestDetail);
                        else
                            existingItem.ItemDetails!.Add(requestDetail);
                    }
                }
                else
                {
                    existingHeader.Items!.Add(requestItem);
                }
            }

            await _context.SaveChangesAsync();
            return existingHeader;
        }

        public async Task<bool> DeleteTemplateAsync(string id)
        {
            var header = await _context.TemplateHeaders.FindAsync(id);
            if (header == null) return false;

            _context.TemplateHeaders.Remove(header);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}