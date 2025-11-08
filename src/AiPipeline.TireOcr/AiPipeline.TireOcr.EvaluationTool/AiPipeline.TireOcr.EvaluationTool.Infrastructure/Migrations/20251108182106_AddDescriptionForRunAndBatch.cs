using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionForRunAndBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EvaluationRuns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EvaluationRunBatches",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "EvaluationRuns");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EvaluationRunBatches");
        }
    }
}
