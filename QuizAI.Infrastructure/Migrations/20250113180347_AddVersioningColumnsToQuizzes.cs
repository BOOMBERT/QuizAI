using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVersioningColumnsToQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeprecated",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LatestVersionId",
                table: "Quizzes",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeprecated",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "LatestVersionId",
                table: "Quizzes");
        }
    }
}
