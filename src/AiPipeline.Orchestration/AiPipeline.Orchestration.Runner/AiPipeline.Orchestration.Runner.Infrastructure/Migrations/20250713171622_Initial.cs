using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.Orchestration.Runner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileStorageScope = table.Column<int>(type: "integer", nullable: false),
                    StorageProvider = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PipelineResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeProcedure",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NodeTypeId = table.Column<string>(type: "text", nullable: false),
                    SchemaVersion = table.Column<int>(type: "integer", nullable: false),
                    InputSchema = table.Column<string>(type: "text", nullable: false),
                    OutputSchema = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeProcedure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeProcedure_NodeTypes_NodeTypeId",
                        column: x => x.NodeTypeId,
                        principalTable: "NodeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineStepResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<string>(type: "text", nullable: false),
                    NodeProcedureId = table.Column<string>(type: "text", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WasSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineStepResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineStepResult_PipelineResults_ResultId",
                        column: x => x.ResultId,
                        principalTable: "PipelineResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeProcedure_NodeTypeId",
                table: "NodeProcedure",
                column: "NodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStepResult_ResultId",
                table: "PipelineStepResult",
                column: "ResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "NodeProcedure");

            migrationBuilder.DropTable(
                name: "PipelineStepResult");

            migrationBuilder.DropTable(
                name: "NodeTypes");

            migrationBuilder.DropTable(
                name: "PipelineResults");
        }
    }
}
