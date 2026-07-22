using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoggingApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                newName: "IX_Logs_WorkspaceId");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Logs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceUsers",
                columns: table => new
                {
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceUsers", x => new { x.WorkspaceId, x.UserId });
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CreatedByUserId",
                table: "Logs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_Name",
                table: "Workspaces",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_OwnerUserId",
                table: "Workspaces",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_UserId",
                table: "WorkspaceUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_WorkspaceId_UserId",
                table: "WorkspaceUsers",
                columns: new[] { "WorkspaceId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_CreatedByUserId",
                table: "Logs",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Workspaces_WorkspaceId",
                table: "Logs",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_CreatedByUserId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Workspaces_WorkspaceId",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "WorkspaceUsers");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Logs_CreatedByUserId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "WorkspaceId",
                table: "Logs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_WorkspaceId",
                table: "Logs",
                newName: "IX_Logs_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
