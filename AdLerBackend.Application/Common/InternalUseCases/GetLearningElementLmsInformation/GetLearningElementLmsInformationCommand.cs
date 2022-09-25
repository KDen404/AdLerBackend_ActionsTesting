using AdLerBackend.Application.Common.Responses.Course;

namespace AdLerBackend.Application.Common.InternalUseCases.GetLearningElementLmsInformation;

public record GetLearningElementLmsInformationCommand : CommandWithToken<GetLearningElementLmsInformationResponse>
{
    public int ElementId { get; init; }
    public int CourseId { get; init; }
}