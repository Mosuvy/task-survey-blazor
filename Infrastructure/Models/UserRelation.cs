using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TaskSurvey.Infrastructure.Models
{
    [Index(nameof(UserId), IsUnique = true)]
    public class UserRelation
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("UserId"), Required]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        [ForeignKey("SupervisorId"), Required]
        public int SupervisorId { get; set; }
        public virtual User? Supervisor { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}