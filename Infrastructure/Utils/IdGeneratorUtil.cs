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
        public static string FormatUserId(int numericId)
        {
            return numericId.ToString("D8");
        }

        public static async Task<string> GetNextFormattedUserId(AppDbContext context)
        {
            var lastId = await context.Users
                .Select(u => u.Id)
                .ToListAsync();

            int maxId = lastId
                .Select(id => int.TryParse(id, out int val) ? val : 0)
                .DefaultIfEmpty(0)
                .Max();

            return FormatUserId(maxId + 1);
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
                .Where(d => d.DocumentId.StartsWith(prefix))
                .OrderByDescending(d => d.DocumentId)
                .Select(d => d.DocumentId)
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