using System.ClientModel;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class RunPodVllmApiTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly IConfiguration _configuration;
    private readonly IPromptRepository _promptRepository;
    private readonly string _modelName;
    private readonly string _endpointName;

    public RunPodVllmApiTireCodeDetectorService(IConfiguration configuration, IPromptRepository promptRepository,
        string modelName, string endpointName)
    {
        _configuration = configuration;
        _promptRepository = promptRepository;
        _modelName = modelName;
        _endpointName = endpointName;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var client = GetChatClient();
            if (client is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    $"Failed to retrieve {_modelName} endpoint configuration"));

            var prompt = await _promptRepository.GetMainPromptAsync();
            List<ChatMessage> messages =
            [
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(prompt),
                    ChatMessageContentPart.CreateImagePart(new BinaryData(image.Data), image.ContentType,
                        ChatImageDetailLevel.High)
                )
            ];
            var options = new ChatCompletionOptions
            {
                Temperature = 0.6f
            };
            var completion = await client.CompleteChatAsync(messages, options);
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
            var failure = new Failure(500,$"Failed to perform Ocr via RunPod vLLM Tire Code Detector with model '{_modelName}'");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private ChatClient? GetChatClient()
    {
        try
        {
            var apiKey = _configuration.GetValue<string>("ApiKeys:RunPod");
            return new ChatClient(
                model: _modelName,
                credential: new ApiKeyCredential(apiKey!),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(_endpointName),
                }
            );
        }
        catch
        {
            return null;
        }
    }
}