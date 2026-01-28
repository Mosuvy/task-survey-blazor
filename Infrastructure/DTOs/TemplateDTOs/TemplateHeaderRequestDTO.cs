using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.TemplateDTOs
{
    public class TemplateHeaderRequestDTO
    {
        public string? Id { get; set; } 
        [Required]
        public string TemplateName { get; set; } = null!;
        public int PositionId { get; set; }
        [Required]
        public string Theme { get; set; } = null!;
        public List<TemplateItemRequestDTO> Items { get; set; } = new();
    }
}