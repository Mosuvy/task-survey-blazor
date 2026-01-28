using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskSurvey.Infrastructure.Models;

namespace TaskSurvey.Infrastructure.DTOs.SurveyDTOs
{
    public class SurveyItemRequestDTO
    {
        public int Id { get; set; }
        [Required]
        public string DocumentSurveyId { get; set; } = null!;
        public int? TemplateItemId { get; set; }
        [Required(ErrorMessage = "Jawaban tidak boleh kosong")]
        public string Answer { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        [Required]
        public string Question { get; set; } = null!;
        [Required]
        public QuestionType Type { get; set; }

        public List<SurveyItemDetailRequestDTO> CheckBox { get; set; } = new();
    }
}