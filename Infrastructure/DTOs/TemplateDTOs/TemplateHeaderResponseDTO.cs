using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.PositionDTOs;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateHeaderResponseDTO
    {
        public required string Id { get; set; }
        public required string TemplateName { get; set; }
        public int PositionId { get; set; }
        public virtual PositionResponseDTO? Position { get; set; }
        public required string Theme { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TemplateItemResponseDTO> Items { get; set; } = new();
    }
}