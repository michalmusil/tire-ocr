using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.Nodes.Dtos.FileReferenceUploader;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceDownloader;
using AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;
using AiPipeline.TireOcr.Shared.Models;
using JasperFx.Core;
using MediatR;
using TireOcr.Preprocessing.Application.Commands.ExtractTireCodeRoi;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.Preprocessing.Messaging.Procedures;

public class PerformTireImagePreprocessingProcedure : IProcedure
{
    public string Id => "PerformTireImagePreprocessing";
    public int SchemaVersion => 1;

    public IApElement InputSchema => _inputOutputSchema;
    public IApElement OutputSchema => _inputOutputSchema;

    private readonly IApElement _inputOutputSchema = new ApObject(
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
            { "image", ApFile.Template(["image/jpeg", "image/png", "image/webp"]) },
        },
        nonRequiredProperties: ["detectorType"]
    );

    private readonly IMediator _mediator;
    private readonly IFileReferenceDownloaderService _fileReferenceDownloaderService;
    private readonly IFileReferenceUploaderService _fileReferenceUploaderService;

    public PerformTireImagePreprocessingProcedure(IMediator mediator,
        IFileReferenceDownloaderService fileReferenceDownloaderService,
        IFileReferenceUploaderService fileReferenceUploaderService)
    {
        _mediator = mediator;
        _fileReferenceDownloaderService = fileReferenceDownloaderService;
        _fileReferenceUploaderService = fileReferenceUploaderService;
    }

    public async Task<DataResult<IApElement>> ExecuteAsync(RunPipelineStep step)
    {
        var input = step.CurrentStepInput;
        var fileReferences = step.FileReferences;

        var schemaIsCompatible = InputSchema.HasCompatibleSchemaWith(input);
        if (!schemaIsCompatible)
            return DataResult<IApElement>.Invalid(
                $"Procedure '{nameof(PerformTireImagePreprocessingProcedure)}' failed: input schema is not compatible");

        var parseResult = ParseValuesFromInput(input);
        if (parseResult.IsFailure)
            return DataResult<IApElement>.Failure(parseResult.Failures);

        TireCodeDetectorType detectorType = parseResult.Data.Item1;
        ApFile inputImage = parseResult.Data.Item2;

        var inputFileReference = fileReferences.FirstOrDefault(f => f.Id == inputImage.Id);
        if (inputFileReference is null)
            return DataResult<IApElement>.Invalid(
                $"Procedure '{nameof(PerformTireImagePreprocessingProcedure)}' failed: missing file reference '{inputImage.Id}'");

        var imageDataResult = await _fileReferenceDownloaderService.DownloadFileReferenceDataAsync(
            reference: inputFileReference,
            userId: step.UserId
        );
        if (imageDataResult.IsFailure)
            return DataResult<IApElement>.Failure(imageDataResult.Failures);

        var imageData = await imageDataResult.Data!.ReadAllBytesAsync();
        var query = new ExtractTireCodeRoiCommand(
            ImageData: imageData,
            ImageName: Path.GetFileName(inputFileReference.Path),
            OriginalContentType: inputFileReference.ContentType,
            EnhanceCharacters: false
        );
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return DataResult<IApElement>.Failure(result.Failures);

        var preprocessedImage = result.Data!;
        var preprocessedImageStream = new MemoryStream(preprocessedImage.ImageData);
        preprocessedImageStream.Position = 0;
        var preprocessedImageId = Guid.NewGuid();

        var uploadParams = new UploadFileParams(
            FileId: preprocessedImageId,
            UserId: step.UserId,
            FileStream: preprocessedImageStream,
            ContentType: preprocessedImage.ContentType,
            FileName: preprocessedImage.Name,
            StorePermanently: false
        );
        var fileReferenceResult = await _fileReferenceUploaderService
            .UploadFileDataAsync(uploadParams);
        if (fileReferenceResult.IsFailure)
            return DataResult<IApElement>.Failure(fileReferenceResult.Failures);

        var newFileReference = fileReferenceResult.Data!;

        fileReferences.Add(newFileReference);
        return DataResult<IApElement>.Success(
            new ApObject(properties: new()
            {
                { "detectorType", new ApEnum(detectorType.ToString(), null) },
                {
                    "image", new ApFile(
                        id: newFileReference.Id,
                        contentType: newFileReference.ContentType
                    )
                }
            })
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
                    $"Procedure '{nameof(PerformTireImagePreprocessingProcedure)}' failed: unsupported detector type '{inputDetectorTypeValue}'");

            return DataResult<(TireCodeDetectorType, ApFile)>.Success(
                ((TireCodeDetectorType)detectorType, (ApFile)inputFile!)
            );
        }
        catch
        {
            return DataResult<(TireCodeDetectorType, ApFile)>.Failure(new Failure(500,
                $"Procedure '{nameof(PerformTireImagePreprocessingProcedure)}' failed unexpectedly: Failed to parse procedure input"));
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