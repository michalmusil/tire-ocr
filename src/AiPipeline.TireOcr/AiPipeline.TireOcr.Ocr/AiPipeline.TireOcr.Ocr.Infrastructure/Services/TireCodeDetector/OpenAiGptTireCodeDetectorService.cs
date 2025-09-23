using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Constants;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class OpenAiGptTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly IConfiguration _configuration;

    public OpenAiGptTireCodeDetectorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var client = GetChatClient();
            if (client is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve OpenAi endpoint configuration"));

            List<ChatMessage> messages =
            [
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(ModelPrompts.TireCodeOcrPrompt),
                    ChatMessageContentPart.CreateImagePart(new BinaryData(image.Data), image.ContentType,
                        ChatImageDetailLevel.High)
                )
            ];
            var completion = await client.CompleteChatAsync(messages);
            var foundTireCode = completion.Value.Content
                .Select(c => c.Text)
                .FirstOrDefault(t => !string.IsNullOrEmpty(t) && t.Contains('/'));

            if (foundTireCode is null)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            string? foundManufacturer = null;
            var indexOfManufacturerSplit = foundTireCode.IndexOf('|');
            var manufacturerFound = indexOfManufacturerSplit > 0;
            if (manufacturerFound)
            {
                foundManufacturer = foundTireCode.Substring(0, indexOfManufacturerSplit);
                foundTireCode = foundTireCode.Substring(indexOfManufacturerSplit + 1);
            }

            var result = new OcrResultDto(
                DetectedTireCode: foundTireCode,
                DetectedManufacturer: foundManufacturer,
                new OcrRequestBillingDto(
                    completion.Value.Usage.InputTokenCount,
                    completion.Value.Usage.OutputTokenCount,
                    BillingUnitType.Token
                )
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, "Failed to perform Ocr via OpenAi Gpt Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private ChatClient? GetChatClient()
    {
        try
        {
            var apiKey = _configuration.GetValue<string>("ApiKeys:OpenAi");
            return new ChatClient("gpt-4o", apiKey);
        }
        catch
        {
            return null;
        }
    }
}