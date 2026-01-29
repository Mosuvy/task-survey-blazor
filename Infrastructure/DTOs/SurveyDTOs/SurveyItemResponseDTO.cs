using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class SurveyItemResponseDTO
    {
        public int Id { get; set; }
        [Required]
        public string DocumentSurveyId { get; set; } = null!;
        public SurveyHeaderResponseDTO? DocumentSurvey { get; set; }
        public int? TemplateItemId { get; set; }
        public TemplateItemResponseDTO? TemplateItem { get; set; }
        [Required]
        public string Answer { get; set; } = null!;
        public int OrderNo { get; set; }
        [Required]
        public string Question { get; set; } = null!;
        public required string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public List<SurveyItemDetailResponseDTO> CheckBox { get; set; } = new();
    }
}