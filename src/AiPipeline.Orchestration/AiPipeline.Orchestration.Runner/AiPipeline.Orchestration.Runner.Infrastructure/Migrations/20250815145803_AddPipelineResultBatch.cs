using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.Orchestration.Runner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPipelineResultBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                table: "PipelineResults",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PipelineResultBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineResultBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineResultBatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineResults_BatchId",
                table: "PipelineResults",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineResultBatches_UserId",
                table: "PipelineResultBatches",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PipelineResults_PipelineResultBatches_BatchId",
                table: "PipelineResults",
                column: "BatchId",
                principalTable: "PipelineResultBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipelineResults_PipelineResultBatches_BatchId",
                table: "PipelineResults");

            migrationBuilder.DropTable(
                name: "PipelineResultBatches");

            migrationBuilder.DropIndex(
                name: "IX_PipelineResults_BatchId",
                table: "PipelineResults");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "PipelineResults");
        }
    }
}
