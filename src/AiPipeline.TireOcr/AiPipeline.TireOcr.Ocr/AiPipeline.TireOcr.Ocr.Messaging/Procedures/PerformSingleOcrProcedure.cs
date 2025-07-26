using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceDownloader;
using AiPipeline.TireOcr.Shared.Models;
using JasperFx.Core;
using MediatR;
using TireOcr.Ocr.Application.Queries.PerformTireImageOcr;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Ocr.Messaging.Procedures;

public class PerformSingleOcrProcedure : IProcedure
{
    public string Id => "PerformSingleOcr";
    public int SchemaVersion => 1;

    public IApElement InputSchema => new ApObject(
        properties: new()
        {
            {
                "detectorType", ApEnum.Template(
                    Enum.GetValues(typeof(TireCodeDetectorType))
                        .Cast<TireCodeDetectorType>()
                        .Select(t => t.ToString())
                        .ToArray()
                )
            },
            {
                "image", ApFile.Template(["image/jpeg", "image/png", "image/webp"])
            }
        }
    );
    public IApElement OutputSchema => new ApString("");

    private readonly IFileReferenceDownloaderService _fileReferenceDownloaderService;
    private readonly IMediator _mediator;

    public PerformSingleOcrProcedure(IFileReferenceDownloaderService fileReferenceDownloaderService, IMediator mediator)
    {
        _fileReferenceDownloaderService = fileReferenceDownloaderService;
        _mediator = mediator;
    }

    public async Task<DataResult<IApElement>> ExecuteAsync(IApElement input, List<FileReference> fileReferences)
    {
        var schemaIsCompatible = InputSchema.HasCompatibleSchemaWith(input);
        if (!schemaIsCompatible)
            return DataResult<IApElement>.Invalid(
                $"Procedure '{nameof(PerformSingleOcrProcedure)}' failed: input schema is not compatible");

        var parseResult = ParseValuesFromInput(input);
        if (parseResult.IsFailure)
            return DataResult<IApElement>.Failure(parseResult.Failures);

        TireCodeDetectorType detectorType = parseResult.Data.Item1;
        ApFile inputFile = parseResult.Data.Item2;

        var inputFileReference = fileReferences.FirstOrDefault(f => f.Id == inputFile.Id);
        if (inputFileReference is null)
            return DataResult<IApElement>.Invalid(
                $"Procedure '{nameof(PerformSingleOcrProcedure)}' failed: missing file reference '{inputFile.Id}'");

        var imageDataResult = await _fileReferenceDownloaderService.DownloadFileReferenceDataAsync(inputFileReference);
        if (imageDataResult.IsFailure)
            return DataResult<IApElement>.Failure(imageDataResult.Failures);

        var imageData = await imageDataResult.Data!.ReadAllBytesAsync();
        var query = new PerformTireImageOcrQuery(
            detectorType,
            imageData,
            Path.GetFileName(inputFileReference.Path),
            inputFile.ContentType
        );

        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: ocrResult => DataResult<IApElement>.Success(new ApString(ocrResult.DetectedCode)),
            onFailure: DataResult<IApElement>.Failure
        );
    }

    private DataResult<(TireCodeDetectorType, ApFile)> ParseValuesFromInput(IApElement input)
    {
        try
        {
            var inputObject = (ApObject)input;
            inputObject.TryGetValueCaseInsensitive("Image", out var inputFile);
            inputObject.TryGetValueCaseInsensitive("DetectorType", out var inputDetectorTypeValue);
            var detectorType = GetTireCodeDetectorTypeFromString(((ApEnum)inputDetectorTypeValue!).Value);
            if (detectorType is null)
                return DataResult<(TireCodeDetectorType, ApFile)>.Invalid(
                    $"Procedure '{nameof(PerformSingleOcrProcedure)}' failed: unsupported detector type '{inputDetectorTypeValue}'");

            return DataResult<(TireCodeDetectorType, ApFile)>.Success(
                ((TireCodeDetectorType)detectorType, (ApFile)inputFile!)
            );
        }
        catch
        {
            return DataResult<(TireCodeDetectorType, ApFile)>.Failure(new Failure(500,
                $"Procedure '{nameof(PerformSingleOcrProcedure)}' failed unexpectedly: Failed to parse procedure input"));
        }
    }

    private TireCodeDetectorType? GetTireCodeDetectorTypeFromString(string value)
    {
        var parsedSuccessfully = Enum.TryParse(typeof(TireCodeDetectorType), value, true, out var detectorType);
        if (!parsedSuccessfully || detectorType is null)
            return null;

        return (TireCodeDetectorType)detectorType;
    }
}