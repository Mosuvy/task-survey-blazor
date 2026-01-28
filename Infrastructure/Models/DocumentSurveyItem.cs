using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class DocumentSurveyItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("DocumentSurveyId")]
        public required string DocumentSurveyId { get; set; }
        public virtual DocumentSurvey? DocumentSurvey { get; set; }
        [ForeignKey("TemplateItemId")]
        public int? TemplateItemId { get; set; }
        public virtual TemplateItem? TemplateItem { get; set; }
        public required string Answer { get; set; }
        public int OrderNo { get; set; }
        public required string Question { get; set; }
        public required QuestionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public virtual ICollection<DocumentItemDetail>? CheckBox { get; set; }
    }
}