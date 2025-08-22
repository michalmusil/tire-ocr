using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.Orchestration.Runner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPipelineResultValueSelector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OutputValueSelector",
                table: "PipelineStepResult",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputValueSelector",
                table: "PipelineStepResult");
        }
    }
}
