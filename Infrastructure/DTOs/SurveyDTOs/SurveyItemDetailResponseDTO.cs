using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class SurveyItemDetailResponseDTO
    {
        public int Id { get; set; }
        public int DocumentItemId { get; set; }
        public virtual SurveyItemResponseDTO? DocumentSurveyItem { get; set; }
        public int? TemplateItemDetailId { get; set; }
        public virtual TemplateItemDetailResponseDTO? TemplateItemDetail { get; set; }
        [Required]
        public string Item { get; set; } = null!;
        public bool IsChecked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}