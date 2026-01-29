using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskSurvey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateHeaders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateHeaders_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemplateItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateHeaderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateItems_TemplateHeaders_TemplateHeaderId",
                        column: x => x.TemplateHeaderId,
                        principalTable: "TemplateHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSurveys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequesterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TemplateHeaderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtTemplate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSurveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSurveys_TemplateHeaders_TemplateHeaderId",
                        column: x => x.TemplateHeaderId,
                        principalTable: "TemplateHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentSurveys_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupervisorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRelations_Users_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRelations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateItemDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateItemId = table.Column<int>(type: "int", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateItemDetails_TemplateItems_TemplateItemId",
                        column: x => x.TemplateItemId,
                        principalTable: "TemplateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSurveyItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentSurveyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateItemId = table.Column<int>(type: "int", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSurveyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSurveyItems_DocumentSurveys_DocumentSurveyId",
                        column: x => x.DocumentSurveyId,
                        principalTable: "DocumentSurveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSurveyItems_TemplateItems_TemplateItemId",
                        column: x => x.TemplateItemId,
                        principalTable: "TemplateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DocumentItemDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentItemId = table.Column<int>(type: "int", nullable: false),
                    TemplateItemDetailId = table.Column<int>(type: "int", nullable: true),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentItemDetails_DocumentSurveyItems_DocumentItemId",
                        column: x => x.DocumentItemId,
                        principalTable: "DocumentSurveyItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentItemDetails_TemplateItemDetails_TemplateItemDetailId",
                        column: x => x.TemplateItemDetailId,
                        principalTable: "TemplateItemDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "CreatedAt", "PositionLevel" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 19, 35, 53, 812, DateTimeKind.Local).AddTicks(5478), "Officer" },
                    { 2, new DateTime(2026, 1, 28, 19, 35, 53, 812, DateTimeKind.Local).AddTicks(5493), "Section Head" },
                    { 3, new DateTime(2026, 1, 28, 19, 35, 53, 812, DateTimeKind.Local).AddTicks(5498), "Departement Head" },
                    { 4, new DateTime(2026, 1, 28, 19, 35, 53, 812, DateTimeKind.Local).AddTicks(5503), "Director" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "RoleName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 19, 35, 53, 812, DateTimeKind.Local).AddTicks(4828), "Supervisor" },
                    { 2, new DateTime(2026, 1, 28, 19, 35, 53, 812, DateTimeKind.Local).AddTicks(4875), "User" }
                });

            migrationBuilder.InsertData(
                table: "TemplateHeaders",
                columns: new[] { "Id", "CreatedAt", "PositionId", "TemplateName", "Theme", "UpdatedAt" },
                values: new object[] { "TEMPLATE/2601/001", new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(13), 3, "Survey Kepuasan Kerja", "Blue Corporate", new DateTime(2026, 1, 28, 19, 35, 53, 941, DateTimeKind.Local).AddTicks(9787) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "PositionId", "PositionName", "RoleId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { "00000001", new DateTime(2026, 1, 28, 19, 35, 53, 934, DateTimeKind.Local).AddTicks(1631), "AQAAAAIAAYagAAAAEFGycd/VVNGFX4l4GO/8SY+UT+izaf9fI0HmQv4oYI84oFhXYoGaaAHG9sjxg3rgHA==", 1, "IT Staff", 2, new DateTime(2026, 1, 28, 19, 35, 53, 934, DateTimeKind.Local).AddTicks(1634), "Andhika" },
                    { "10000001", new DateTime(2026, 1, 28, 19, 35, 53, 934, DateTimeKind.Local).AddTicks(1570), "AQAAAAIAAYagAAAAEFGycd/VVNGFX4l4GO/8SY+UT+izaf9fI0HmQv4oYI84oFhXYoGaaAHG9sjxg3rgHA==", 3, "Departement Leader", 1, new DateTime(2026, 1, 28, 19, 35, 53, 934, DateTimeKind.Local).AddTicks(1615), "Wahyu Johan" },
                    { "10000002", new DateTime(2026, 1, 28, 19, 35, 53, 934, DateTimeKind.Local).AddTicks(1624), "AQAAAAIAAYagAAAAEFGycd/VVNGFX4l4GO/8SY+UT+izaf9fI0HmQv4oYI84oFhXYoGaaAHG9sjxg3rgHA==", 3, "Departement Leader", 1, new DateTime(2026, 1, 28, 19, 35, 53, 934, DateTimeKind.Local).AddTicks(1627), "Edi" }
                });

            migrationBuilder.InsertData(
                table: "DocumentSurveys",
                columns: new[] { "Id", "CreatedAt", "RequesterId", "Status", "TemplateHeaderId", "UpdatedAt", "UpdatedAtTemplate" },
                values: new object[] { "SURVEY/2601/0001", new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(318), "00000001", 0, "TEMPLATE/2601/001", new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(322), new DateTime(2026, 1, 28, 19, 35, 53, 941, DateTimeKind.Local).AddTicks(9787) });

            migrationBuilder.InsertData(
                table: "TemplateItems",
                columns: new[] { "Id", "CreatedAt", "OrderNo", "Question", "TemplateHeaderId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(129), 1, "Apa pendapat Anda tentang lingkungan kerja?", "TEMPLATE/2601/001", 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(133) },
                    { 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(139), 2, "Fasilitas yang Anda gunakan:", "TEMPLATE/2601/001", 1, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(142) }
                });

            migrationBuilder.InsertData(
                table: "UserRelations",
                columns: new[] { "Id", "CreatedAt", "SupervisorId", "UserId" },
                values: new object[] { 1, new DateTime(2026, 1, 28, 19, 35, 53, 941, DateTimeKind.Local).AddTicks(9416), "10000001", "00000001" });

            migrationBuilder.InsertData(
                table: "DocumentSurveyItems",
                columns: new[] { "Id", "Answer", "CreatedAt", "DocumentSurveyId", "OrderNo", "Question", "TemplateItemId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Lingkungan kerja sangat kondusif dan mendukung produktivitas.", new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(409), "SURVEY/2601/0001", 1, "Apa pendapat Anda tentang lingkungan kerja?", 1, 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(412) },
                    { 2, "Selected", new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(431), "SURVEY/2601/0001", 2, "Fasilitas yang Anda gunakan:", 2, 1, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(434) }
                });

            migrationBuilder.InsertData(
                table: "TemplateItemDetails",
                columns: new[] { "Id", "CreatedAt", "Item", "TemplateItemId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(223), "Laptop Inventaris", 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(236) },
                    { 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(240), "Ruang Meeting", 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(242) }
                });

            migrationBuilder.InsertData(
                table: "DocumentItemDetails",
                columns: new[] { "Id", "CreatedAt", "DocumentItemId", "IsChecked", "Item", "TemplateItemDetailId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(637), 2, true, "Laptop Inventaris", 1, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(642) },
                    { 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(647), 2, true, "Ruang Meeting", 2, new DateTime(2026, 1, 28, 19, 35, 53, 942, DateTimeKind.Local).AddTicks(649) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentItemDetails_DocumentItemId",
                table: "DocumentItemDetails",
                column: "DocumentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentItemDetails_TemplateItemDetailId",
                table: "DocumentItemDetails",
                column: "TemplateItemDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyItems_DocumentSurveyId",
                table: "DocumentSurveyItems",
                column: "DocumentSurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyItems_TemplateItemId",
                table: "DocumentSurveyItems",
                column: "TemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveys_RequesterId",
                table: "DocumentSurveys",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveys_TemplateHeaderId",
                table: "DocumentSurveys",
                column: "TemplateHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateHeaders_PositionId",
                table: "TemplateHeaders",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateItemDetails_TemplateItemId",
                table: "TemplateItemDetails",
                column: "TemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateItems_TemplateHeaderId",
                table: "TemplateItems",
                column: "TemplateHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_SupervisorId",
                table: "UserRelations",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_UserId",
                table: "UserRelations",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PositionId",
                table: "Users",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentItemDetails");

            migrationBuilder.DropTable(
                name: "UserRelations");

            migrationBuilder.DropTable(
                name: "DocumentSurveyItems");

            migrationBuilder.DropTable(
                name: "TemplateItemDetails");

            migrationBuilder.DropTable(
                name: "DocumentSurveys");

            migrationBuilder.DropTable(
                name: "TemplateItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TemplateHeaders");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}
