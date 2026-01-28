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
                    { 1, new DateTime(2026, 1, 28, 15, 50, 5, 344, DateTimeKind.Local).AddTicks(9665), "Officer" },
                    { 2, new DateTime(2026, 1, 28, 15, 50, 5, 344, DateTimeKind.Local).AddTicks(9669), "Section Head" },
                    { 3, new DateTime(2026, 1, 28, 15, 50, 5, 344, DateTimeKind.Local).AddTicks(9671), "Departement Head" },
                    { 4, new DateTime(2026, 1, 28, 15, 50, 5, 344, DateTimeKind.Local).AddTicks(9673), "Director" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "RoleName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 15, 50, 5, 344, DateTimeKind.Local).AddTicks(9372), "Supervisor" },
                    { 2, new DateTime(2026, 1, 28, 15, 50, 5, 344, DateTimeKind.Local).AddTicks(9415), "User" }
                });

            migrationBuilder.InsertData(
                table: "TemplateHeaders",
                columns: new[] { "Id", "CreatedAt", "PositionId", "TemplateName", "Theme", "UpdatedAt" },
                values: new object[] { "TEMPLATE/2601/001", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4780), 3, "Survey Kepuasan Kerja", "Blue Corporate", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4750) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "PositionId", "PositionName", "RoleId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { "00000001", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4573), "AQAAAAIAAYagAAAAEE7eW/Q+Ajn/TvkVzTYCTLrnZwudxv3pXPlY2R0tiPz0s65qBMst7fYI2o+keRMlIg==", 3, "Departement Leader", 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4607), "Wahyu Johan" },
                    { "00000002", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4618), "AQAAAAIAAYagAAAAEE7eW/Q+Ajn/TvkVzTYCTLrnZwudxv3pXPlY2R0tiPz0s65qBMst7fYI2o+keRMlIg==", 3, "Departement Leader", 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4619), "Edi" },
                    { "00000003", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4622), "AQAAAAIAAYagAAAAEE7eW/Q+Ajn/TvkVzTYCTLrnZwudxv3pXPlY2R0tiPz0s65qBMst7fYI2o+keRMlIg==", 1, "IT Staff", 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4623), "Andhika" }
                });

            migrationBuilder.InsertData(
                table: "DocumentSurveys",
                columns: new[] { "Id", "CreatedAt", "RequesterId", "Status", "TemplateHeaderId", "UpdatedAt", "UpdatedAtTemplate" },
                values: new object[] { "SURVEY/2601/0001", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4896), "00000003", 0, "TEMPLATE/2601/001", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4897), new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4750) });

            migrationBuilder.InsertData(
                table: "TemplateItems",
                columns: new[] { "Id", "CreatedAt", "OrderNo", "Question", "TemplateHeaderId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4817), 1, "Apa pendapat Anda tentang lingkungan kerja?", "TEMPLATE/2601/001", 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4821) },
                    { 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4824), 2, "Fasilitas yang Anda gunakan:", "TEMPLATE/2601/001", 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4825) }
                });

            migrationBuilder.InsertData(
                table: "UserRelations",
                columns: new[] { "Id", "CreatedAt", "SupervisorId", "UserId" },
                values: new object[] { 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4742), "00000001", "00000003" });

            migrationBuilder.InsertData(
                table: "DocumentSurveyItems",
                columns: new[] { "Id", "Answer", "CreatedAt", "DocumentSurveyId", "OrderNo", "Question", "TemplateItemId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Lingkungan kerja sangat kondusif dan mendukung produktivitas.", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4927), "SURVEY/2601/0001", 1, "Apa pendapat Anda tentang lingkungan kerja?", 1, 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4929) },
                    { 2, "Selected", new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4932), "SURVEY/2601/0001", 2, "Fasilitas yang Anda gunakan:", 2, 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4933) }
                });

            migrationBuilder.InsertData(
                table: "TemplateItemDetails",
                columns: new[] { "Id", "CreatedAt", "Item", "TemplateItemId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4856), "Laptop Inventaris", 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4858) },
                    { 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4860), "Ruang Meeting", 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4861) }
                });

            migrationBuilder.InsertData(
                table: "DocumentItemDetails",
                columns: new[] { "Id", "CreatedAt", "DocumentItemId", "IsChecked", "Item", "TemplateItemDetailId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4970), 2, true, "Laptop Inventaris", 1, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4972) },
                    { 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4975), 2, true, "Ruang Meeting", 2, new DateTime(2026, 1, 28, 15, 50, 5, 415, DateTimeKind.Local).AddTicks(4976) }
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
