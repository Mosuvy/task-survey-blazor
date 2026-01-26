using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class TemplateHeader
    {
        [Key, Required]
        public required string Id { get; set; }
        public required string TemplateName { get; set; }
        [ForeignKey("PositionId"), Required]
        public int PositionId { get; set; }
        public virtual Position? Position { get; set; }
        public required string Theme { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<TemplateItem>? Items { get; set; }
        public virtual ICollection<DocumentSurvey>? Surveys { get; set; }
    }
}