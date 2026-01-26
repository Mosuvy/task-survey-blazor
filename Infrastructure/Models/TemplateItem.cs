using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class TemplateItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("TemplateHeaderId")]
        public required string TemplateHeaderId { get; set; }
        public virtual TemplateHeader? TemplateHeader { get; set; }
        public required string Question { get; set; }
        public required QuestionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<TemplateItemDetail>? ItemDetails { get; set; }
        public virtual ICollection<DocumentSurveyItem>? SurveyItems { get; set; }
    }

    public enum QuestionType
    {
        TextBox = 0,
        CheckBox = 1,
        TextArea = 2,
        Number = 3
    }
}