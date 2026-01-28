using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class SurveyItemDetailRequestDTO
    {
        public int Id { get; set; }

        public int DocumentItemId { get; set; }

        public int? TemplateItemDetailId { get; set; }

        [Required]
        public string Item { get; set; } = null!;

        public bool IsChecked { get; set; }
    }
}