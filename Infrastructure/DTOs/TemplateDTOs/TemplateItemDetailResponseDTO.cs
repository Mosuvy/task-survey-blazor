using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateItemDetailResponseDTO
    {
        public int Id { get; set; }
        public int TemplateItemId { get; set; }
        public virtual TemplateItemResponseDTO? TemplateItem { get; set; }
        public required string Item { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}