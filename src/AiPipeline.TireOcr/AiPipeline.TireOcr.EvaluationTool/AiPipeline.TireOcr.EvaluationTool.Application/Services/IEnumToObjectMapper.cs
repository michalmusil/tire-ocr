using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface IEnumToObjectMapper<in TEnum, TObject>
    where TEnum : Enum
    where TObject : class
{
    public DataResult<TObject> Map(TEnum input);
}