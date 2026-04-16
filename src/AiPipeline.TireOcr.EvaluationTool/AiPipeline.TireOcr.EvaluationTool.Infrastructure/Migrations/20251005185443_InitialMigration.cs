using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationRunBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationRunBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    InputImage_FileName = table.Column<string>(type: "text", nullable: false),
                    InputImage_ContentType = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PreprocessingType = table.Column<int>(type: "integer", nullable: false),
                    OcrType = table.Column<int>(type: "integer", nullable: false),
                    PostprocessingType = table.Column<int>(type: "integer", nullable: false),
                    DbMatchingType = table.Column<int>(type: "integer", nullable: false),
                    RunFailure_Reason = table.Column<int>(type: "integer", nullable: true),
                    RunFailure_Code = table.Column<int>(type: "integer", nullable: true),
                    RunFailure_Message = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationRuns_EvaluationRunBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "EvaluationRunBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DbMatchingResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RunId = table.Column<Guid>(type: "uuid", nullable: false),
                    Matches = table.Column<string>(type: "text", nullable: false),
                    ManufacturerMatch = table.Column<string>(type: "text", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbMatchingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbMatchingResults_EvaluationRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "EvaluationRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RunId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpectedTireCode_RawCode = table.Column<string>(type: "text", nullable: false),
                    ExpectedTireCode_VehicleClass = table.Column<string>(type: "text", nullable: true),
                    ExpectedTireCode_Width = table.Column<decimal>(type: "numeric", nullable: true),
                    ExpectedTireCode_AspectRatio = table.Column<decimal>(type: "numeric", nullable: true),
                    ExpectedTireCode_Construction = table.Column<string>(type: "text", nullable: true),
                    ExpectedTireCode_Diameter = table.Column<decimal>(type: "numeric", nullable: true),
                    ExpectedTireCode_LoadRange = table.Column<char>(type: "character(1)", nullable: true),
                    ExpectedTireCode_LoadIndex = table.Column<int>(type: "integer", nullable: true),
                    ExpectedTireCode_LoadIndex2 = table.Column<int>(type: "integer", nullable: true),
                    ExpectedTireCode_SpeedRating = table.Column<string>(type: "text", nullable: true),
                    TotalDistance = table.Column<int>(type: "integer", nullable: false),
                    FullMatchParameterCount = table.Column<int>(type: "integer", nullable: false),
                    EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: false),
                    VehicleClassEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    VehicleClassEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    WidthEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    WidthEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    DiameterEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    DiameterEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    AspectRatioEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    AspectRatioEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    ConstructionEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    ConstructionEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    LoadRangeEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    LoadRangeEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    LoadIndexEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    LoadIndexEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    LoadIndex2Evaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    LoadIndex2Evaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    SpeedRatingEvaluation_Distance = table.Column<int>(type: "integer", nullable: true),
                    SpeedRatingEvaluation_EstimatedAccuracy = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_EvaluationRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "EvaluationRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OcrResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RunId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetectedCode = table.Column<string>(type: "text", nullable: false),
                    DetectedManufacturer = table.Column<string>(type: "text", nullable: true),
                    InputUnitCount = table.Column<decimal>(type: "numeric", nullable: true),
                    OutputUnitCount = table.Column<decimal>(type: "numeric", nullable: true),
                    BillingUnit = table.Column<string>(type: "text", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "numeric", nullable: true),
                    EstimatedCostCurrency = table.Column<string>(type: "text", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcrResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcrResults_EvaluationRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "EvaluationRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostprocessingResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RunId = table.Column<Guid>(type: "uuid", nullable: false),
                    TireCode_RawCode = table.Column<string>(type: "text", nullable: false),
                    TireCode_VehicleClass = table.Column<string>(type: "text", nullable: true),
                    TireCode_Width = table.Column<decimal>(type: "numeric", nullable: true),
                    TireCode_AspectRatio = table.Column<decimal>(type: "numeric", nullable: true),
                    TireCode_Construction = table.Column<string>(type: "text", nullable: true),
                    TireCode_Diameter = table.Column<decimal>(type: "numeric", nullable: true),
                    TireCode_LoadRange = table.Column<char>(type: "character(1)", nullable: true),
                    TireCode_LoadIndex = table.Column<int>(type: "integer", nullable: true),
                    TireCode_LoadIndex2 = table.Column<int>(type: "integer", nullable: true),
                    TireCode_SpeedRating = table.Column<string>(type: "text", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostprocessingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostprocessingResults_EvaluationRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "EvaluationRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreprocessingResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RunId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreprocessingResult_FileName = table.Column<string>(type: "text", nullable: false),
                    PreprocessingResult_ContentType = table.Column<string>(type: "text", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreprocessingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreprocessingResults_EvaluationRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "EvaluationRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbMatchingResults_RunId",
                table: "DbMatchingResults",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationRuns_BatchId",
                table: "EvaluationRuns",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_RunId",
                table: "Evaluations",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OcrResults_RunId",
                table: "OcrResults",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostprocessingResults_RunId",
                table: "PostprocessingResults",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreprocessingResults_RunId",
                table: "PreprocessingResults",
                column: "RunId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbMatchingResults");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "OcrResults");

            migrationBuilder.DropTable(
                name: "PostprocessingResults");

            migrationBuilder.DropTable(
                name: "PreprocessingResults");

            migrationBuilder.DropTable(
                name: "EvaluationRuns");

            migrationBuilder.DropTable(
                name: "EvaluationRunBatches");
        }
    }
}
