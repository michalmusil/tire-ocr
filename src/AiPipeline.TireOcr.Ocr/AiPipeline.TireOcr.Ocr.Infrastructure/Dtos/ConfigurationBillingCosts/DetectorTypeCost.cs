using AiPipeline.TireOcr.Shared.Models;

namespace TireOcr.Ocr.Infrastructure.Dtos.ConfigurationBillingCosts;

public class DetectorTypeCost
{
    public TireCodeDetectorType DetectorType { get; set; }
    public decimal AmountInUsd { get; set; }
}