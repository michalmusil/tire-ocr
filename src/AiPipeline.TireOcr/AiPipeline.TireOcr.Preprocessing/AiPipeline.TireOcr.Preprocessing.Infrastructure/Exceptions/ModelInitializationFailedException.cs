namespace TireOcr.Preprocessing.Infrastructure.Exceptions;

public class ModelInitializationFailedException(
    string serviceName,
    string failureReason,
    Exception? innerException = null)
    : Exception(
        $"Failed to initialize underlying ML model for service '{serviceName}'. Failure reason: '{failureReason}'",
        innerException)
{
}