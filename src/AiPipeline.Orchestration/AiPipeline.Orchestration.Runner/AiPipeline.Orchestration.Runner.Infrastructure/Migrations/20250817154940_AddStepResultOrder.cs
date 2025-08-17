using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.Orchestration.Runner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStepResultOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PipelineStepResult",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "PipelineStepResult");
        }
    }
}
