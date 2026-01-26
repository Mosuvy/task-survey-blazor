using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public required string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}