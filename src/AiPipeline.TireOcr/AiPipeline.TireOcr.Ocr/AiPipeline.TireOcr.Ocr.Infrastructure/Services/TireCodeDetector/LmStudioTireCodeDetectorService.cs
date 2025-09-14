using System.ClientModel;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Constants;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class LmStudioTireCodeDetectorService : ITireCodeDetectorService
{
    private const string Model = "google/gemma-3-4b";
    private readonly IConfiguration _configuration;

    public LmStudioTireCodeDetectorService(IConfiguration configuration)
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
                    "Failed to retrieve LmStudio endpoint configuration"));

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

            var result = new OcrResultDto(
                foundTireCode,
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
            var failure = new Failure(500, "Failed to perform Ocr via Lm Studio Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private ChatClient? GetChatClient()
    {
        try
        {
            var endpoint = _configuration.GetValue<string>("OcrEndpoints:LmStudio");
            return new ChatClient(Model, new ApiKeyCredential("dummyCredential"), new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint!)
            });
        }
        catch
        {
            return null;
        }
    }
}