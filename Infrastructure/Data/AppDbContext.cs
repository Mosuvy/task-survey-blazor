using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Models;
using TaskSurvey.Infrastructure.Utils;

namespace TaskSurvey.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<TemplateHeader> TemplateHeaders { get; set; } = null!;
        public DbSet<TemplateItem> TemplateItems { get; set; } = null!;
        public DbSet<TemplateItemDetail> TemplateItemDetails { get; set; } = null!;
        public DbSet<DocumentSurvey> DocumentSurveys { get; set; } = null!;
        public DbSet<DocumentSurveyItem> DocumentSurveyItems { get; set; } = null!;
        public DbSet<DocumentItemDetail> DocumentItemDetails { get; set; } = null!;
        public DbSet<UserRelation> UserRelations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRelation>(entity =>
            {
                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.SupervisorRelations)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ur => ur.Supervisor)
                    .WithMany(u => u.SubordinateRelations)
                    .HasForeignKey(ur => ur.SupervisorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TemplateItem>(entity =>
            {
                entity.HasOne(ti => ti.TemplateHeader)
                    .WithMany(th => th.Items)
                    .HasForeignKey(ti => ti.TemplateHeaderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TemplateItemDetail>(entity =>
            {
                entity.HasOne(tid => tid.TemplateItem)
                    .WithMany(ti => ti.ItemDetails)
                    .HasForeignKey(tid => tid.TemplateItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DocumentSurvey>(entity =>
            {
                entity.HasOne(ds => ds.Requester)
                    .WithMany(u => u.Documents)
                    .HasForeignKey(ds => ds.RequesterId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(ds => ds.Header)
                    .WithMany(th => th.Surveys)
                    .HasForeignKey(ds => ds.TemplateHeaderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DocumentSurveyItem>(entity =>
            {
                entity.HasOne(di => di.DocumentSurvey)
                    .WithMany(ds => ds.SurveyItems)
                    .HasForeignKey(di => di.DocumentSurveyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(di => di.TemplateItem)
                    .WithMany(ti => ti.SurveyItems)
                    .HasForeignKey(di => di.TemplateItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DocumentItemDetail>(entity =>
            {
                entity.HasOne(did => did.DocumentSurveyItem)
                    .WithMany(di => di.CheckBox)
                    .HasForeignKey(did => did.DocumentItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(did => did.TemplateItemDetail)
                    .WithMany(tid => tid.ItemDetails)
                    .HasForeignKey(did => did.TemplateItemDetailId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Position)
                    .WithMany(p => p.Users)
                    .HasForeignKey(u => u.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TemplateHeader>()
                .HasOne(th => th.Position)
                .WithMany(p => p.Headers)
                .HasForeignKey(th => th.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    RoleName = "Supervisor",
                    CreatedAt = DateTime.Now
                },
                new Role
                {
                    Id = 2,
                    RoleName = "User",  
                    CreatedAt = DateTime.Now
                }
            );
            modelBuilder.Entity<Position>().HasData(
                new Position
                {
                    Id = 1,
                    PositionLevel = "Officer",
                    CreatedAt = DateTime.Now
                },
                new Position
                {
                    Id = 2,
                    PositionLevel = "Section Head",
                    CreatedAt = DateTime.Now
                },
                new Position
                {
                    Id = 3,
                    PositionLevel = "Departement Head",
                    CreatedAt = DateTime.Now
                },
                new Position
                {
                    Id = 4,
                    PositionLevel = "Director",
                    CreatedAt = DateTime.Now
                }
            );
            var password = PasswordUtil.HashPassword("password");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Wahyu Johan",
                    PasswordHash = password,
                    PositionId = 3,
                    PositionName = "Departement Leader",
                    RoleId = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );
        }
    }
}