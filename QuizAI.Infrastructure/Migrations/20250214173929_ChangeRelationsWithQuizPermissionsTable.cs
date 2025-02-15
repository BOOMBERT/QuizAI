using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationsWithQuizPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizPermissions_QuizId",
                table: "QuizPermissions");

            migrationBuilder.DropIndex(
                name: "IX_QuizPermissions_UserId",
                table: "QuizPermissions");

            migrationBuilder.CreateIndex(
                name: "IX_QuizPermissions_QuizId",
                table: "QuizPermissions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizPermissions_UserId",
                table: "QuizPermissions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizPermissions_QuizId",
                table: "QuizPermissions");

            migrationBuilder.DropIndex(
                name: "IX_QuizPermissions_UserId",
                table: "QuizPermissions");

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
    }
}
