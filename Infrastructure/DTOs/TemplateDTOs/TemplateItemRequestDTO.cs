using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.SurveyDTOs;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateItemRequestDTO
    {
        public int Id { get; set; } 
        public required string Question { get; set; }
        public required string Type { get; set; }
        public int OrderNo { get; set; }
        public List<TemplateItemDetailRequestDTO> ItemDetails { get; set; } = new();
    }
}