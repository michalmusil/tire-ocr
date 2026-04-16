using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var preprocessingService = builder.AddProject<AiPipeline_TireOcr_Preprocessing_WebApi>("PreprocessingService")
    .WithHttpHealthCheck("/health");

var ocrService = builder.AddProject<AiPipeline_TireOcr_Ocr_WebApi>("OcrService")
    .WithHttpHealthCheck("/health");

var postprocessingService = builder.AddProject<AiPipeline_TireOcr_Postprocessing_WebApi>("PostprocessingService")
    .WithHttpHealthCheck("/health");

var tireDbMatcherService = builder.AddProject<AiPipeline_TireOcr_DbMatcher_WebApi>("DbMatcherService")
    .WithHttpHealthCheck("/health");

var pythonPreprocessingService = builder.AddUvicornApp("PreprocessingPythonService",
        "../AiPipeline.TireOcr.PythonPreprocessing", "main:app")
    .WithPip()
    .WithExternalHttpEndpoints();

var pythonOcrService = builder.AddUvicornApp("OcrPythonService",
        "../AiPipeline.TireOcr.PythonOcr", "main:app")
    .WithPip()
    .WithExternalHttpEndpoints();

var evaluationTool = builder.AddProject<AiPipeline_TireOcr_EvaluationTool_WebApi>("EvaluationTool")
    .WithHttpHealthCheck("/health")
    .WithReference(preprocessingService)
    .WaitFor(preprocessingService)
    .WithReference(ocrService)
    .WaitFor(ocrService)
    .WithReference(postprocessingService)
    .WaitFor(postprocessingService)
    .WithReference(tireDbMatcherService)
    .WaitFor(tireDbMatcherService)
    .WithReference(pythonOcrService)
    .WaitFor(pythonOcrService)
    .WithReference(pythonPreprocessingService)
    .WaitFor(pythonPreprocessingService);

var frontend = builder.AddViteApp("Frontend", "../AiPipeline.TireOcr.EvaluationTool.Ui")
    .WithNpm()
    .WithExternalHttpEndpoints()
    .WithEnvironment("EVALUATION_VITE_PROXY_URL", () =>
    {
        var endpoint = evaluationTool.GetEndpoint("https").Exists
            ? evaluationTool.GetEndpoint("https").Url
            : evaluationTool.GetEndpoint("http").Url;
        return endpoint;
    });
;


builder.Build().Run();