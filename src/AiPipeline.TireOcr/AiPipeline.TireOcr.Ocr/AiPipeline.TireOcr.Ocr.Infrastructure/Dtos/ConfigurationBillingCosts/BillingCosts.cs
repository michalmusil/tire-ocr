namespace TireOcr.Ocr.Infrastructure.Dtos.ConfigurationBillingCosts;

public class BillingCosts
{
    public const string Key = "BillingCosts";

    public required List<DetectorTypeCost> InputUnitCosts { get; set; }
    public required List<DetectorTypeCost> OutputUnitCosts { get; set; }
}