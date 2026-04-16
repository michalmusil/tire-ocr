using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Shared.Result;
using Image = TireOcr.Ocr.Domain.ImageEntity.Image;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class GoogleCloudVisionTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly IConfiguration _configuration;

    public GoogleCloudVisionTireCodeDetectorService(IConfiguration configuration)
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
                    "Failed to retrieve Google Cloud vision endpoint configuration"));

            var imageToSend = Google.Cloud.Vision.V1.Image.FromBytes(image.Data);
            var detectedTexts = await client.DetectTextAsync(imageToSend);

            var rawResult = string.Join(
                ' ',
                detectedTexts.Select(dt => dt.Description)
            );

            var tireCodeFound = rawResult.Contains('/') && rawResult.Any(char.IsDigit);
            if (!tireCodeFound)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            var result = new OcrResultDto(
                DetectedTireCode: rawResult,
                DetectedManufacturer: null,
                new OcrRequestBillingDto(0, 1, BillingUnitType.Transaction)
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, "Failed to perform Ocr via OpenAi Gpt Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private ImageAnnotatorClient? GetOcrClient()
    {
        try
        {
            var jsonCredentials = _configuration.GetValue<string>("ApiKeys:GcpJsonCredentials")!;
            var clientBuilder = new ImageAnnotatorClientBuilder
            {
                JsonCredentials = jsonCredentials,
            };
            return clientBuilder.Build();
        }
        catch
        {
            return null;
        }
    }
}