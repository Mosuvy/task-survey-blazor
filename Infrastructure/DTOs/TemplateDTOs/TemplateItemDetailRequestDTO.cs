using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class TemplateItemDetailRequestDTO
    {
        public int Id { get; set; }
        [Required]
        public string Item { get; set; } = null!;
    }
}