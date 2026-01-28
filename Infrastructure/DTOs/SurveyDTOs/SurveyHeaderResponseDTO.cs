using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.DTOs.TemplateDTOs;
using TaskSurvey.Infrastructure.DTOs.UserDTOs;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class SurveyHeaderResponseDTO
    {
        [Required]
        public string DocumentId { get; set; } = null!;
        [Required]
        public string RequesterId { get; set; } = null!;
        public virtual UserResponseDTO? Requester { get; set; }
        public required string Status { get; set; }
        public string? TemplateHeaderId { get; set; }
        public virtual TemplateHeaderResponseDTO? Header { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime UpdatedAtTemplate { get; set; }

        public List<SurveyItemResponseDTO> SurveyItems { get; set; } = new();
    }
}