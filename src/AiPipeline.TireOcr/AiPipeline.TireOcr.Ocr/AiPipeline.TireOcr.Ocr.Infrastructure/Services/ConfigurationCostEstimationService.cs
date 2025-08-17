using AiPipeline.TireOcr.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Infrastructure.Dtos.ConfigurationBillingCosts;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services;

public class ConfigurationCostEstimationService : ICostEstimationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationCostEstimationService> _logger;

    public ConfigurationCostEstimationService(IConfiguration configuration,
        ILogger<ConfigurationCostEstimationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<DataResult<EstimatedCostsDto>> GetEstimatedCostsAsync(TireCodeDetectorType detectorType,
        OcrRequestBillingDto billing)
    {
        var billingCosts = GetBillingCostsFromConfiguration();

        var inputUnitPrice = billingCosts.InputUnitCosts
            .FirstOrDefault(uc => uc.DetectorType == detectorType)
            ?.AmountInUsd;
        var outputUnitPrice = billingCosts.OutputUnitCosts
            .FirstOrDefault(uc => uc.DetectorType == detectorType)
            ?.AmountInUsd;
        if (inputUnitPrice is null || outputUnitPrice is null)
        {
            var message =
                $"{nameof(ConfigurationCostEstimationService)}: Failed to get unit costs for detector type {detectorType}";
            _logger.LogCritical(message);
            return DataResult<EstimatedCostsDto>.NotFound(message);
        }

        var totalCost = billing.InputAmount * inputUnitPrice + billing.OutputAmount * outputUnitPrice;

        var costsDto = new EstimatedCostsDto(
            InputUnitCount: billing.InputAmount,
            OutputUnitCount: billing.OutputAmount,
            BillingUnit: billing.UnitType.ToString(),
            EstimatedCost: totalCost.Value,
            EstimatedCostCurrency: "USD"
        );

        return DataResult<EstimatedCostsDto>.Success(costsDto);
    }

    private BillingCosts GetBillingCostsFromConfiguration()
    {
        var billingCosts = new BillingCosts
        {
            InputUnitCosts = [],
            OutputUnitCosts = []
        };
        _configuration.GetSection(BillingCosts.Key).Bind(billingCosts);
        if (billingCosts.InputUnitCosts.Count == 0 || billingCosts.OutputUnitCosts.Count == 0)
            _logger.LogCritical(
                $"{nameof(ConfigurationCostEstimationService)}: Failed to load billing costs from configuration");

        return billingCosts;
    }
}