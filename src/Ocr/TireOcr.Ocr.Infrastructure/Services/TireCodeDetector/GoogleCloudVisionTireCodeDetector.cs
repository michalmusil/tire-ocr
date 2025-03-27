using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Shared.Result;
using Image = TireOcr.Ocr.Domain.ImageEntity.Image;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class GoogleCloudVisionTireCodeDetector : ITireCodeDetector
{
    private readonly IConfiguration _configuration;

    public GoogleCloudVisionTireCodeDetector(IConfiguration configuration)
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
                    "Failed to retrieve OpenAi endpoint configuration"));

            var imageToSend = Google.Cloud.Vision.V1.Image.FromBytes(image.Data);
            var detectedTexts = await client.DetectTextAsync(imageToSend);

            var foundTireCode = detectedTexts
                .Select(dt => dt.Description)
                .FirstOrDefault(t => !string.IsNullOrEmpty(t) && t.Contains('/'));

            if (foundTireCode is null)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            var result = new OcrResultDto(foundTireCode);
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
            var credentialsRelativePath = _configuration.GetValue<string>("ApiKeys:GcpJsonCredentialsRelativePath")!;
            var credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, credentialsRelativePath);
            
            var clientBuilder = new ImageAnnotatorClientBuilder()
            {
                CredentialsPath = credPath,
            };
            return clientBuilder.Build();
        }
        catch
        {
            return null;
        }
    }
}