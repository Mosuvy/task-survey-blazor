using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class SurveyHeaderRequestDTO
    {
        [Required]
        public string DocumentId { get; set; } = null!;
        [Required(ErrorMessage = "Requester harus diisi")]
        public string RequesterId { get; set; } = null!;
        public StatusType Status { get; set; } = StatusType.Draft;
        public string? TemplateHeaderId { get; set; }

        public List<SurveyItemRequestDTO> SurveyItems { get; set; } = new();
    }
}