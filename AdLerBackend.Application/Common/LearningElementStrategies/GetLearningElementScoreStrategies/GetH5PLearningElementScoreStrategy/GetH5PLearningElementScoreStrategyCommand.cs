using AdLerBackend.Application.Common.Responses.LearningElements;
using AdLerBackend.Application.Common.Responses.LMSAdapter;

#pragma warning disable CS8618
namespace AdLerBackend.Application.Common.LearningElementStrategies.GetLearningElementScoreStrategies.
    GetH5PLearningElementScoreStrategy;

public record GetH5PLearningElementScoreStrategyCommand : CommandWithToken<LearningElementScoreResponse>
{
    public int ElementId { get; init; }
    public Modules LearningElementMoule { get; init; }
}