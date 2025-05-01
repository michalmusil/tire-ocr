using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class OpenAiGptTireCodeDetector : ITireCodeDetector
{
    private readonly IConfiguration _configuration;

    private readonly string _prompt =
        "In the following image, there should be a picture of a portion of a car tire. On this picture, there should be embossed tire code. The format is for example: \"185/75R1482S\" or \"P215/55ZR1895V\". Keep in mind that the \"/\" character has to be in the output. Please read the tire code from the image and return only the detected code string itself (for example just \"210/60ZR15\"). If you can't detect any tire code in the photo for whatever reason, just answer with letter \"N\" and nothing else.";


    public OpenAiGptTireCodeDetector(IConfiguration configuration)
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
                    ChatMessageContentPart.CreateTextPart(_prompt),
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
                new OcrRequestBillingDto(completion.Value.Usage.TotalTokenCount, BillingUnit.Token)
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