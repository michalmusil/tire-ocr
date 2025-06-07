using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services.CostEstimation;

public interface ICostEstimationService
{
    public Task<DataResult<EstimatedCostsDto>> GetEstimatedCostsAsync(
        TireCodeDetectorType detectorType,
        BillingDto billing
    );
}