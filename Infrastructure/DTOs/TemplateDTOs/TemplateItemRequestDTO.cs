using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateItemRequestDTO
    {
        public int Id { get; set; } 
        [Required]
        public string Question { get; set; } = null!;
        [Required]
        public QuestionType Type { get; set; }
        public int OrderNo { get; set; }
        public List<TemplateItemDetailRequestDTO> ItemDetails { get; set; } = new();
    }
}