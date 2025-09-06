using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.Orchestration.Runner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNodeIdToProcedurePk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NodeProcedure",
                table: "NodeProcedure");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NodeProcedure",
                table: "NodeProcedure",
                columns: new[] { "Id", "NodeTypeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NodeProcedure",
                table: "NodeProcedure");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NodeProcedure",
                table: "NodeProcedure",
                column: "Id");
        }
    }
}
