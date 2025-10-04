using Microsoft.AspNetCore.Mvc;
using TireOcr.RunnerPrototype.Contracts.RunBatch;
using TireOcr.RunnerPrototype.Contracts.RunSingle;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Dtos.Batch;
using TireOcr.RunnerPrototype.Extensions;
using TireOcr.RunnerPrototype.Models;
using TireOcr.RunnerPrototype.Services.PipelineRunner;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Controllers;

[ApiController]
[Route("[controller]")]
public class RunController : ControllerBase
{
    private readonly IPipelineRunnerService _pipelineRunnerService;
    private readonly ILogger<RunController> _logger;

    public RunController(IPipelineRunnerService pipelineRunnerService, ILogger<RunController> logger)
    {
        _pipelineRunnerService = pipelineRunnerService;
        _logger = logger;
    }

    /// <summary>
    /// Processes a single tire image by sequentially performing preprocessing, ocr, postprocessing and db matching pipeline steps. 
    /// </summary>
    /// <remarks>
    /// This endpoint is designed to analyze an uploaded image of a tire photo, extract general parameters of the tire,
    /// match them against existing tire code variations stored in a database and return the results in a structured
    /// format. Ensuring that the whole tire is visible in the photo, the tire code is not obscured, the tire is well
    /// lighted and the tire in the photo is not skewed (perspective of the photographer is roughly 90° in each
    /// direction) will ensure the best possible results.
    ///
    /// ### Expected inputs
    /// * A tire photo with aforementioned qualities in one of following formats: [jpg, jpeg, png, webp]
    /// * Type of detector to use for the Ocr step of the pipeline, one of: [GoogleGemini, MistralPixtral, OpenAiGpt, GoogleCloudVision, AzureAiVision]
    /// 
    /// ### Extracted parameters
    /// * Vehicle class - 1 or 2 letters, usually not present
    /// * Width - width of the tire in millimeters
    /// * Aspect ratio - aspect ratio of the sidewall height to the tire width
    /// * Construction - 1 letter indicating the construction type of the tire
    /// * Diameter - diameter of the tire in inches (rarely in millimeters)
    /// * Load range - a single letter indicating tire ply rating (only rarely present, only on light truck tires)
    /// * Load index - a whole number, indicates load index of the tire for a normal tire, or single-rear-wheel load index for light truck tire
    /// * Load index 2 - a whole number, only present in light truck tires - indicates dual-rear-wheel load index 
    /// * Speed rating - 1 letter (sometimes with an additional number) indicating the speed rating of the tire
    ///
    /// ### Request format
    /// A multipart/form-data request is expected with the following parts:
    /// * A specification of the detector type
    /// ```
    /// ------exampleBoundary123
    /// Content-Disposition: form-data; name="DetectorType"
    ///
    /// GoogleGemini
    /// ```
    /// * Input photo of the tire
    /// ```
    /// ------exampleBoundary123
    /// Content-Disposition: form-data; name="Image"; filename="example-image.jpg"
    /// Content-Type: image/jpeg
    ///
    /// **raw image bytes**
    /// ```
    /// </remarks>
    /// <param name="request">The request object containing the image and detector type.</param>
    /// <returns>A response object with the OCR results containing decoded tire parameters, estimated variable costs of processing and details of each pipeline steps.</returns>
    /// <response code="200">Returns the processed results.</response>
    /// <response code="400">If the request payload is invalid.</response>
    /// <response code="404">If no tire code is found in one of the pipeline steps and the pipeline can't continue.</response>
    /// <response code="422">If the request payload is valid but unprocessable (e.g., unsupported content type, content not recognized).</response>
    [HttpPost]
    [Consumes("multipart/form-data")] // Explicitly define the content type for form data
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunSingleResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunSingleResponse>> Single([FromForm] RunSingleRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var imageToProcess = new Image(imageData, request.Image.FileName, request.Image.ContentType);

        var result = await _pipelineRunnerService.RunSingleOcrPipelineAsync(imageToProcess, request.DetectorType);

        return result.ToActionResult<TireOcrResultDto, RunSingleResponse>(
            onSuccess: res => new RunSingleResponse(res)
        );
    }

    /// <summary>
    /// Processes a multiple tire images in sequentially executed batches. 
    /// </summary>
    /// <remarks>
    /// This endpoint is designed to execute the whole OCR pipeline on a collection of multiple tire photos. It is provided
    /// mainly for statistics purposes. Execution of this endpoint may take a long time depending on number of provided
    /// images, so it is recommended to turn timeouts off on any client.
    ///
    /// ### Expected inputs
    /// * A list of URLs leading to images with qualities and formats specified in the `Run` endpoint.
    /// * Type of detector to use for running the pipeline on all provided images.
    /// * Batch size - maximum value is 10. Provided image URLS are processed in batches. Batches are run sequentially,
    /// but images within batch are processed in parallel. Maximum batch size is set to prevent preprocessing pipeline step
    /// overload, as this step is demanding on computation power.
    /// 
    ///
    /// ### Request format
    /// A multipart/form-data request is expected with the following parts:
    /// * A specification of the detector type
    /// ```
    /// ------exampleBoundary123
    /// Content-Disposition: form-data; name="DetectorType"
    ///
    /// GoogleGemini
    /// ```
    /// * A specification of the batch size (max 10)
    /// ```
    /// ------exampleBoundary123
    /// Content-Disposition: form-data; name="BatchSize"
    ///
    /// 2
    /// ```
    /// * An entry for each provided image url
    /// ```
    /// ------exampleBoundary123
    /// Content-Disposition: form-data; name="ImageUrls"
    ///
    /// https://example.com/example-image.jpeg
    /// ```
    /// </remarks>
    /// <returns>A response object with the OCR results for each processed image (either successful or failure) as well as processing statistics summary.</returns>
    /// <response code="200">Returns the processed results.</response>
    /// <response code="400">If the request payload is invalid.</response>
    /// <response code="422">If the request payload is valid but unprocessable (e.g., unsupported content type, content not recognized).</response>
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunBatchResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunBatchResponse>> Batch([FromForm] RunBatchRequest request)
    {
        var result = await _pipelineRunnerService.RunOcrPipelineBatchAsync(
            request.ImageUrls,
            request.BatchSize,
            request.DetectorType
        );

        return result.ToActionResult<TireOcrBatchResultDto, RunBatchResponse>(
            onSuccess: res => new RunBatchResponse(res)
        );
    }
}