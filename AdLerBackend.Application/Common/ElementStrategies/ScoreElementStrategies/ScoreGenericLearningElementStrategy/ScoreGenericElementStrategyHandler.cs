using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.Responses.Elements;
using MediatR;

namespace AdLerBackend.Application.Common.ElementStrategies.ScoreElementStrategies.
    ScoreGenericLearningElementStrategy;

public class ScoreGenericElementStrategyHandler : IRequestHandler<ScoreGenericElementStrategyCommand,
    ScoreElementResponse>
{
    private readonly ILMS _lms;

    public ScoreGenericElementStrategyHandler(ILMS lms)
    {
        _lms = lms;
    }

    public async Task<ScoreElementResponse> Handle(ScoreGenericElementStrategyCommand request,
        CancellationToken cancellationToken)
    {
        return new ScoreElementResponse
        {
            IsSuccess = await _lms.ScoreGenericElement(request.WebServiceToken, request.Module.Id)
        };
    }
}