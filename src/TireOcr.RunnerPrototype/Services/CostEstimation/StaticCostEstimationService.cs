using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services.CostEstimation;

public class StaticCostEstimationService : ICostEstimationService
{
    private readonly Dictionary<TireCodeDetectorType, decimal> _usDollarInputUnitCosts = new()
    {
        { TireCodeDetectorType.GoogleCloudVision, 0m },
        { TireCodeDetectorType.AzureAiVision, 0m },
        { TireCodeDetectorType.GoogleGemini, 0.0000001m },
        { TireCodeDetectorType.OpenAiGpt, 0.0000025m },
        { TireCodeDetectorType.MistralPixtral, 0.000002m },
    };

    private readonly Dictionary<TireCodeDetectorType, decimal> _usDollarOutputUnitCosts = new()
    {
        { TireCodeDetectorType.GoogleCloudVision, 0.0015m },
        { TireCodeDetectorType.AzureAiVision, 0.0015m },
        { TireCodeDetectorType.GoogleGemini, 0.0000004m },
        { TireCodeDetectorType.OpenAiGpt, 0.00001m },
        { TireCodeDetectorType.MistralPixtral, 0.000006m },
    };

    public async Task<DataResult<EstimatedCostsDto>> GetEstimatedCostsAsync(
        TireCodeDetectorType detectorType,
        BillingDto billing
    )
    {
        var inputUnitPrice = _usDollarInputUnitCosts.GetValueOrDefault(detectorType);
        var outputUnitPrice = _usDollarOutputUnitCosts.GetValueOrDefault(detectorType);
        var totalCost = billing.InputAmount * inputUnitPrice + billing.OutputAmount * outputUnitPrice;

        var costsDto = new EstimatedCostsDto(
            billing.InputAmount,
            billing.OutputAmount,
            billing.Unit,
            totalCost,
            "$"
        );

        return DataResult<EstimatedCostsDto>.Success(costsDto);
    }
}