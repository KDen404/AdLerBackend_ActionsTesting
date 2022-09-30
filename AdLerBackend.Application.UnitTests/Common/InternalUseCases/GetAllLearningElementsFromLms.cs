using AdLerBackend.Application.Common.Exceptions;
using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.InternalUseCases.GetAllLearningElementsFromLms;
using AdLerBackend.Application.Common.Responses.Course;
using AdLerBackend.Application.Common.Responses.LMSAdapter;
using AdLerBackend.Domain.Entities;
using NSubstitute;

namespace AdLerBackend.Application.UnitTests.Common.InternalUseCases;

public class GetAllLearningElementsFromLms
{
    private ICourseRepository _courseRepository;
    private IFileAccess _fileAccess;
    private IMoodle _moodle;
    private ISerialization _serialization;


    [SetUp]
    public void Setup()
    {
        _moodle = Substitute.For<IMoodle>();
        _courseRepository = Substitute.For<ICourseRepository>();
        _fileAccess = Substitute.For<IFileAccess>();
        _serialization = Substitute.For<ISerialization>();
    }

    [Test]
    public async Task GetLearningElementLmsInformation_Valid_RetunsCourseModule()
    {
        // Arrange
        var systemUnderTest =
            new GetAllLearningElementsFromLmsHandler(_courseRepository, _fileAccess, _serialization, _moodle);

        _courseRepository.GetAsync(Arg.Any<int>()).Returns(new CourseEntity
        {
            Id = 2,
            Name = "name",
            AuthorId = 1234,
            DslLocation = "asd",
            H5PFilesInCourse = new List<H5PLocationEntity>
            {
                new()
                {
                    Id = 3,
                    Path = "path",
                    ElementId = 4
                }
            }
        });

        _fileAccess.GetFileStream(Arg.Any<string>()).Returns(new MemoryStream());
        _serialization.GetObjectFromJsonStreamAsync<LearningWorldDtoResponse>(Arg.Any<Stream>())
            .Returns(new LearningWorldDtoResponse
            {
                LearningWorld = new LearningWorld
                {
                    LearningElements = new List<Application.Common.Responses.Course.LearningElement>
                    {
                        new()
                        {
                            Id = 1337,
                            Identifier = new Identifier
                            {
                                Value = "searchedFileName"
                            }
                        }
                    }
                }
            });

        _moodle.SearchCoursesAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new MoodleCourseListResponse
        {
            Courses = new List<MoodleCourse>
            {
                new()
                {
                    Id = 1
                }
            }
        });

        _moodle.GetCourseContentAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new[]
        {
            new CourseContent
            {
                Modules = new List<Modules>
                {
                    new()
                    {
                        Name = "searchedFileName",
                        contextid = 123
                    }
                }
            }
        });

        // Act

        var result =
            await systemUnderTest.Handle(new GetAllLearningElementsFromLmsCommand
            {
                CourseId = 1,
                WebServiceToken = "token"
            }, CancellationToken.None);


        // Assert

        Assert.NotNull(result);
    }


    [Test]
    public async Task GetLearningElementLmsInformation_CourseNotFound_Throws()
    {
        // Arrange
        var systemUnderTest =
            new GetAllLearningElementsFromLmsHandler(_courseRepository, _fileAccess, _serialization, _moodle);

        _courseRepository.GetAsync(Arg.Any<int>()).Returns((CourseEntity?) null);

        _fileAccess.GetFileStream(Arg.Any<string>()).Returns(new MemoryStream());
        _serialization.GetObjectFromJsonStreamAsync<LearningWorldDtoResponse>(Arg.Any<Stream>())
            .Returns(new LearningWorldDtoResponse
            {
                LearningWorld = new LearningWorld
                {
                    LearningElements = new List<Application.Common.Responses.Course.LearningElement>
                    {
                        new()
                        {
                            Id = 1337,
                            Identifier = new Identifier
                            {
                                Value = "searchedFileName"
                            }
                        }
                    }
                }
            });

        _moodle.SearchCoursesAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new MoodleCourseListResponse
        {
            Courses = new List<MoodleCourse>
            {
                new()
                {
                    Id = 1
                }
            }
        });

        _moodle.GetCourseContentAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new[]
        {
            new CourseContent
            {
                Modules = new List<Modules>
                {
                    new()
                    {
                        Name = "searchedFileName",
                        contextid = 123
                    }
                }
            }
        });

        // Act
        //Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await systemUnderTest.Handle(new GetAllLearningElementsFromLmsCommand
            {
                CourseId = 1,
                WebServiceToken = "token"
            }, CancellationToken.None));
    }
}