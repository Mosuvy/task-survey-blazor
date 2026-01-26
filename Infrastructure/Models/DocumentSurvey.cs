using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class DocumentSurvey
    {
        [Key]
        public required string DocumentId { get; set; }
        [ForeignKey("RequesterId"), Required]
        public required string RequesterId { get; set; }
        public virtual User? Requester { get; set; }
        public StatusType Status { get; set; }
        [ForeignKey("TemplateHeaderId")]
        public required string TemplateHeaderId { get; set; }
        public virtual TemplateHeader? Header { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<DocumentSurveyItem>? SurveyItems { get; set; }
    }

    public enum StatusType
    {
        Draft = 0,
        ConfirmToApprove = 1,
        Confirmed = 2,
    }
}