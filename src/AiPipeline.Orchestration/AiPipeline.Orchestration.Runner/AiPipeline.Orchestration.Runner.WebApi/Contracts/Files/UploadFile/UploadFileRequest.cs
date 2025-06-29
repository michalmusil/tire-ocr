using System.ComponentModel.DataAnnotations;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.UploadFile;

public record UploadFileRequest(
    [Required] IFormFile File,
    Guid? Id
);