using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSurvey.Infrastructure.Models
{
    public class User
    {
        [Key, Required]
        public string Id { get; set; } = null!;
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        [ForeignKey("PositionId")]
        public int PositionId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Position? Position { get; set; }
        public required string PositionName { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Role? Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<UserRelation>? SupervisorRelations { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<UserRelation>? SubordinateRelations { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<DocumentSurvey>? Documents { get; set; }
    }
}