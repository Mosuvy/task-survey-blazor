using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateHeaderRequestDTO
    {
        public string? Id { get; set; } 
        public required string TemplateName { get; set; }
        public int PositionId { get; set; }
        public required string Theme { get; set; }
        public List<TemplateItemRequestDTO> Items { get; set; } = new();
    }
}