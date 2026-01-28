using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class DocumentItemDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("DocumentItemId")]
        public int DocumentItemId { get; set; }
        public virtual DocumentSurveyItem? DocumentSurveyItem { get; set; }
        [ForeignKey("TemplateItemDetailId")]
        public int? TemplateItemDetailId { get; set; }
        public virtual TemplateItemDetail? TemplateItemDetail { get; set; }
        public required string Item { get; set; }
        public bool IsChecked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}