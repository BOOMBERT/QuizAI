using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizPermissionsTableAndIsPrivateColumnToQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "QuizPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanPlay = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizPermissions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizPermissions_QuizId",
                table: "QuizPermissions",
                column: "QuizId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizPermissions_UserId",
                table: "QuizPermissions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizPermissions");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Quizzes");
        }
    }
}
