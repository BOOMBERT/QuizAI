using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipsTrueFalseAnswerOpenEndedAnswerToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrueFalseAnswers_QuestionId",
                table: "TrueFalseAnswers");

            migrationBuilder.DropIndex(
                name: "IX_OpenEndedAnswers_QuestionId",
                table: "OpenEndedAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_TrueFalseAnswers_QuestionId",
                table: "TrueFalseAnswers",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenEndedAnswers_QuestionId",
                table: "OpenEndedAnswers",
                column: "QuestionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrueFalseAnswers_QuestionId",
                table: "TrueFalseAnswers");

            migrationBuilder.DropIndex(
                name: "IX_OpenEndedAnswers_QuestionId",
                table: "OpenEndedAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_TrueFalseAnswers_QuestionId",
                table: "TrueFalseAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenEndedAnswers_QuestionId",
                table: "OpenEndedAnswers",
                column: "QuestionId");
        }
    }
}
