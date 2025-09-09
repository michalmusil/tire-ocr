using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class AzureAiVisionTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly IConfiguration _configuration;

    public AzureAiVisionTireCodeDetectorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var client = GetOcrClient();
            if (client is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Azure AI vision configuration"));

            var imageToSend = new BinaryData(image.Data);
            var detectionResult = await client.AnalyzeAsync(
                imageToSend,
                VisualFeatures.Read
            );

            var foundTireCode = detectionResult.Value.Read.Blocks
                .SelectMany(b => b.Lines)
                .Select(l => l.Text)
                .Where(s => s.Contains('/') && s.Any(char.IsDigit))
                .MaxBy(s => s.Length);

            if (foundTireCode is null)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            var result = new OcrResultDto(
                foundTireCode,
                new OcrRequestBillingDto(0, 1, BillingUnitType.Transaction)
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, "Failed to perform Ocr via Azure AI Vision Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private ImageAnalysisClient? GetOcrClient()
    {
        try
        {
            var endpoint = _configuration.GetValue<string>("OcrEndpoints:Azure")!;
            var key = _configuration.GetValue<string>("ApiKeys:AzureAiVision")!;

            return new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
        }
        catch
        {
            return null;
        }
    }
}