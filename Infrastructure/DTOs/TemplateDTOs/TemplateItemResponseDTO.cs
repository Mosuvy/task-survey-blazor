using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateItemResponseDTO
    {
        public int Id { get; set; }
        public required string TemplateHeaderId { get; set; }
        public virtual TemplateHeaderResponseDTO? TemplateHeader { get; set; }
        public required string Question { get; set; }
        public required string Type { get; set; }
        public int OrderNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<TemplateItemDetailResponseDTO> ItemDetails { get; set; } = new();
    }
}