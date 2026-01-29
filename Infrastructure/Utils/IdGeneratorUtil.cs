using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;

namespace TaskSurvey.Infrastructure.Utils
{
    public class IdGeneratorUtil
    {
        public static string FormatUserId(int numericId, bool isSupervisor)
        {
            string prefix = isSupervisor ? "1" : "0";
            
            return prefix + numericId.ToString("D7");
        }

        public static async Task<string> GetNextFormattedUserId(AppDbContext context, bool isSupervisor)
        {
            string prefixChar = isSupervisor ? "1" : "0";

            var existingIds = await context.Users
                .Where(u => u.Id.StartsWith(prefixChar))
                .Select(u => u.Id)
                .ToListAsync();

            int maxId = 0;

            if (existingIds.Any())
            {
                maxId = existingIds
                    .Select(id => 
                    {
                        string suffix = id.Substring(1); 
                        return int.TryParse(suffix, out int val) ? val : 0;
                    })
                    .Max();
            }

            return FormatUserId(maxId + 1, isSupervisor);
        }

        public static async Task<string> GenerateTemplateId(AppDbContext context)
        {
            var now = DateTime.Now;
            string datePart = now.ToString("yyMM");
            string prefix = $"TEMPLATE/{datePart}/";

            var lastId = await context.TemplateHeaders
                .Where(t => t.Id.StartsWith(prefix))
                .OrderByDescending(t => t.Id)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastId != null)
            {
                string lastPart = lastId.Split('/').Last();
                if (int.TryParse(lastPart, out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            return $"{prefix}{nextNumber:D3}"; // TEMPLATE/2601/001
        }

        public static async Task<string> GenerateSurveyId(AppDbContext context)
        {
            var now = DateTime.Now;
            string datePart = now.ToString("yyMM");
            string prefix = $"SURVEY/{datePart}/";

            var lastId = await context.DocumentSurveys
                .Where(d => d.Id.StartsWith(prefix))
                .OrderByDescending(d => d.Id)
                .Select(d => d.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastId != null)
            {
                string lastPart = lastId.Split('/').Last();
                if (int.TryParse(lastPart, out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}"; // SURVEY/2601/0001
        }
    }
}