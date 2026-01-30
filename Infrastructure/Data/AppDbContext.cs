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
                    .OnDelete(DeleteBehavior.Cascade);

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
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ds => ds.Header)
                    .WithMany(th => th.Surveys)
                    .HasForeignKey(ds => ds.TemplateHeaderId)
                    .OnDelete(DeleteBehavior.SetNull);
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
                    .OnDelete(DeleteBehavior.SetNull);
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
                    .OnDelete(DeleteBehavior.SetNull);
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
                },
                new Role
                {
                    Id = 3,
                    RoleName = "Overseer",
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
                    Id = "00000001",
                    Username = "Wahyu Johan",
                    PasswordHash = password,
                    PositionId = 1,
                    PositionName = "Officer Leader",
                    RoleId = 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new User
                {
                    Id = "00000002",
                    Username = "Edi",
                    PasswordHash = password,
                    PositionId = 3,
                    PositionName = "Departement Leader",
                    RoleId = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new User
                {
                    Id = "00000003",
                    Username = "Andhika",
                    PasswordHash = password,
                    PositionId = 1,
                    PositionName = "IT Staff",
                    RoleId = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<UserRelation>().HasData(
                new UserRelation
                {
                    Id = 1,
                    UserId = "00000003",
                    SupervisorId = "00000001",
                    CreatedAt = DateTime.Now
                },
                new UserRelation
                {
                    Id = 2,
                    UserId = "00000001",
                    SupervisorId = "00000002",
                    CreatedAt = DateTime.Now
                }
            );
            var templateTime = DateTime.Now;

            modelBuilder.Entity<TemplateHeader>().HasData(
                new TemplateHeader
                {
                    Id = "TEMPLATE/2601/001",
                    TemplateName = "Template Survey 1",
                    PositionId = 1,
                    Theme = "Survey Kepuasan Kerja",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = templateTime
                }
            );

            modelBuilder.Entity<TemplateItem>().HasData(
                new TemplateItem
                {
                    Id = 1,
                    TemplateHeaderId = "TEMPLATE/2601/001",
                    Question = "Apa pendapat Anda tentang lingkungan kerja?",
                    Type = QuestionType.TextArea,
                    OrderNo = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new TemplateItem
                {
                    Id = 2,
                    TemplateHeaderId = "TEMPLATE/2601/001",
                    Question = "Fasilitas yang Anda gunakan:",
                    Type = QuestionType.CheckBox,
                    OrderNo = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<TemplateItemDetail>().HasData(
                new TemplateItemDetail
                {
                    Id = 1,
                    TemplateItemId = 2,
                    Item = "Laptop Inventaris",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new TemplateItemDetail
                {
                    Id = 2,
                    TemplateItemId = 2,
                    Item = "Ruang Meeting",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<DocumentSurvey>().HasData(
                new DocumentSurvey
                {
                    Id = "SURVEY/2601/0001",
                    RequesterId = "00000003",
                    Status = StatusType.Draft,
                    TemplateHeaderId = "TEMPLATE/2601/001",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UpdatedAtTemplate = templateTime
                }
            );

            modelBuilder.Entity<DocumentSurveyItem>().HasData(
                new DocumentSurveyItem
                {
                    Id = 1,
                    DocumentSurveyId = "SURVEY/2601/0001",
                    TemplateItemId = 1,
                    Question = "Apa pendapat Anda tentang lingkungan kerja?",
                    Type = QuestionType.TextArea,
                    OrderNo = 1,
                    Answer = "Lingkungan kerja sangat kondusif dan mendukung produktivitas.",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new DocumentSurveyItem
                {
                    Id = 2,
                    DocumentSurveyId = "SURVEY/2601/0001",
                    TemplateItemId = 2,
                    Question = "Fasilitas yang Anda gunakan:",
                    Type = QuestionType.CheckBox,
                    OrderNo = 2,
                    Answer = "",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<DocumentItemDetail>().HasData(
                new DocumentItemDetail
                {
                    Id = 1,
                    DocumentItemId = 2,
                    TemplateItemDetailId = 1,
                    Item = "Laptop Inventaris",
                    IsChecked = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new DocumentItemDetail
                {
                    Id = 2,
                    DocumentItemId = 2,
                    TemplateItemDetailId = 2,
                    Item = "Ruang Meeting",
                    IsChecked = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );
        }
    }
}