using AdLerBackend.Application.Common.Exceptions;
using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.Responses.World;
using MediatR;

namespace AdLerBackend.Application.Common.InternalUseCases.GetAllElementsFromLms;

public class
    GetAllElementsFromLmsUseCase : IRequestHandler<GetAllElementsFromLmsCommand,
        GetAllElementsFromLmsWithAdLerIdResponse>
{
    private readonly IFileAccess _fileAccess;
    private readonly ILMS _lms;
    private readonly ISerialization _serialization;
    private readonly IWorldRepository _worldRepository;

    public GetAllElementsFromLmsUseCase(IWorldRepository worldRepository, IFileAccess fileAccess,
        ISerialization serialization, ILMS lms)
    {
        _worldRepository = worldRepository;
        _fileAccess = fileAccess;
        _serialization = serialization;
        _lms = lms;
    }

    public async Task<GetAllElementsFromLmsWithAdLerIdResponse> Handle(GetAllElementsFromLmsCommand request,
        CancellationToken cancellationToken)
    {
        // Get Course from DB
        var course = await _worldRepository.GetAsync(request.WorldId) ??
                     throw new NotFoundException($"Course with the Id {request.WorldId} not found");

        // Get ATF File
        await using var fileStream = _fileAccess.GetReadFileStream(course.DslLocation);
        var dslObject = await _serialization.GetObjectFromJsonStreamAsync<WorldDtoResponse>(fileStream);


        var searchedCourse = await _lms.SearchWorldsAsync(request.WebServiceToken, course.Name);
        var courseContent = await _lms.GetWorldContentAsync(request.WebServiceToken, searchedCourse.Courses[0].Id);

        // Get the LMS-Modules by their name
        var modulesWithId = dslObject.World.Elements.Select(x =>
        {
            var module = courseContent.SelectMany(c => c.Modules)
                .FirstOrDefault(m => m.Name == x.LmsElementIdentifier.Value);

            // If no Module is found just return nothing
            if (module == null) return new ModuleWithId {IsLocked = true, AdLerId = x.ElementId};

            return new ModuleWithId {AdLerId = x.ElementId!, LmsModule = module, IsLocked = false};
        }).ToList();

        return new GetAllElementsFromLmsWithAdLerIdResponse
        {
            ModulesWithAdLerId = modulesWithId,
            LmsCourseId = searchedCourse.Courses[0].Id
        };
    }
}