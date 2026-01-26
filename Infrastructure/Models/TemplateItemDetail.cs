using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class TemplateItemDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("TemplateItemId"), Required]
        public int TemplateItemId { get; set; }
        public virtual TemplateItem? TemplateItem { get; set; }
        public required string Item { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<DocumentItemDetail>? ItemDetails { get; set; }
    }
}