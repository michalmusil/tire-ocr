using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class OpenAiGptTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly IConfiguration _configuration;
    private readonly IPromptRepository _promptRepository;
    private readonly ILogger<OpenAiGptTireCodeDetectorService> _logger;

    public OpenAiGptTireCodeDetectorService(IConfiguration configuration, IPromptRepository promptRepository,
        ILogger<OpenAiGptTireCodeDetectorService> logger)
    {
        _configuration = configuration;
        _promptRepository = promptRepository;
        _logger = logger;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var client = GetChatClient();
            if (client is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve OpenAi endpoint configuration"));

            var prompt = await _promptRepository.GetMainPromptAsync();
            List<ChatMessage> messages =
            [
                new SystemChatMessage(
                    ChatMessageContentPart.CreateTextPart(prompt)
                ),
                new UserChatMessage(
                    ChatMessageContentPart.CreateImagePart(new BinaryData(image.Data),
                        image.ContentType,
                        ChatImageDetailLevel.High
                    )
                )
            ];
            var options = new ChatCompletionOptions
            {
                Temperature = 0.6f
            };
            _logger.LogInformation("Start ocr via OpenAi Gpt Tire Code Detector");
            var completion = await client.CompleteChatAsync(messages, options);
            var foundTireCode = completion.Value.Content
                .Select(c => c.Text)
                .FirstOrDefault(t => !string.IsNullOrEmpty(t) && t.Contains('/'));
            _logger.LogInformation(
                $"Finished ocr via OpenAi Gpt Tire Code Detector, completions: '{string.Join(' ', completion.Value.Content.Select(c => c.Text))}'"
            );

            if (foundTireCode is null)
            {
                _logger.LogWarning($"OpenAi: No tire code detected, finish reason: '{completion.Value.FinishReason}', refusal: '{completion.Value.Refusal}'");
                return DataResult<OcrResultDto>.NotFound("No tire code detected");
            }

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
            return new ChatClient("gpt-4.1", apiKey);
        }
        catch
        {
            return null;
        }
    }
}