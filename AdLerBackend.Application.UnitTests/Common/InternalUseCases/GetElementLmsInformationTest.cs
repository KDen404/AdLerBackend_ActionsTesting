using AdLerBackend.Application.Common.Exceptions;
using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.InternalUseCases.GetElementLmsInformation;
using AdLerBackend.Application.Common.Responses.Course;
using AdLerBackend.Application.Common.Responses.LMSAdapter;
using AdLerBackend.Domain.Entities;
using NSubstitute;

namespace AdLerBackend.Application.UnitTests.Common.InternalUseCases;

public class GerElementLmsInformationTest
{
    private IFileAccess _fileAccess;
    private ILMS _ilms;
    private ISerialization _serialization;
    private IWorldRepository _worldRepository;

    [SetUp]
    public void Setup()
    {
        _ilms = Substitute.For<ILMS>();
        _worldRepository = Substitute.For<IWorldRepository>();
        _fileAccess = Substitute.For<IFileAccess>();
        _serialization = Substitute.For<ISerialization>();
    }

    [Test]
    public async Task GetLearningElementLmsInformation_Valid_RetunsCourseModule()
    {
        // Arrange
        var systemUnderTest =
            new GetLearningElementLmsInformationHandler(_ilms, _worldRepository, _fileAccess, _serialization);

        _worldRepository.GetAsync(Arg.Any<int>()).Returns(new WorldEntity
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
        _serialization.GetObjectFromJsonStreamAsync<WorldDtoResponse>(Arg.Any<Stream>())
            .Returns(new WorldDtoResponse
            {
                LearningWorld = new LearningWorld
                {
                    LearningElements = new List<LearningElement>
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

        _ilms.SearchWorldsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new LMSWorldListResponse
        {
            Courses = new List<MoodleCourse>
            {
                new()
                {
                    Id = 1
                }
            }
        });

        _ilms.GetWorldContentAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new[]
        {
            new WorldContent
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
            await systemUnderTest.Handle(new GetElementLmsInformationCommand
            {
                WorldId = 1,
                ElementId = 1337,
                WebServiceToken = "token"
            }, CancellationToken.None);


        // Assert

        Assert.NotNull(result);
    }

    [Test]
    public async Task GetLearningElementLmsInformation_WrongCourseId_Throws()
    {
        // Arrange
        var systemUnderTest =
            new GetLearningElementLmsInformationHandler(_ilms, _worldRepository, _fileAccess, _serialization);

        _worldRepository.GetAsync(Arg.Any<int>()).Returns((WorldEntity?) null);

        _fileAccess.GetFileStream(Arg.Any<string>()).Returns(new MemoryStream());
        _serialization.GetObjectFromJsonStreamAsync<WorldDtoResponse>(Arg.Any<Stream>())
            .Returns(new WorldDtoResponse
            {
                LearningWorld = new LearningWorld
                {
                    LearningElements = new List<LearningElement>
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

        _ilms.SearchWorldsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new LMSWorldListResponse
        {
            Courses = new List<MoodleCourse>
            {
                new()
                {
                    Id = 1
                }
            }
        });

        _ilms.GetWorldContentAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new[]
        {
            new WorldContent
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
            await systemUnderTest.Handle(new GetElementLmsInformationCommand
            {
                WorldId = 1,
                ElementId = 13333337,
                WebServiceToken = "token"
            }, CancellationToken.None));
    }

    [Test]
    public async Task GetLearningElementLmsInformation_WrongElementId_Throws()
    {
        // Arrange
        var systemUnderTest =
            new GetLearningElementLmsInformationHandler(_ilms, _worldRepository, _fileAccess, _serialization);

        _worldRepository.GetAsync(Arg.Any<int>()).Returns((WorldEntity?) null);

        _fileAccess.GetFileStream(Arg.Any<string>()).Returns(new MemoryStream());
        _serialization.GetObjectFromJsonStreamAsync<WorldDtoResponse>(Arg.Any<Stream>())
            .Returns(new WorldDtoResponse
            {
                LearningWorld = new LearningWorld
                {
                    LearningElements = new List<LearningElement>
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

        _ilms.SearchWorldsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new LMSWorldListResponse
        {
            Courses = new List<MoodleCourse>
            {
                new()
                {
                    Id = 1
                }
            }
        });

        _ilms.GetWorldContentAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new[]
        {
            new WorldContent
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
            await systemUnderTest.Handle(new GetElementLmsInformationCommand
            {
                WorldId = 1,
                ElementId = 13333337,
                WebServiceToken = "token"
            }, CancellationToken.None));
    }


    [Test]
    public async Task GetLearningElementLmsInformation_CorruptedFileName_Throws()
    {
        // Arrange
        var systemUnderTest =
            new GetLearningElementLmsInformationHandler(_ilms, _worldRepository, _fileAccess, _serialization);

        _worldRepository.GetAsync(Arg.Any<int>()).Returns(new WorldEntity
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
        _serialization.GetObjectFromJsonStreamAsync<WorldDtoResponse>(Arg.Any<Stream>())
            .Returns(new WorldDtoResponse
            {
                LearningWorld = new LearningWorld
                {
                    LearningElements = new List<LearningElement>
                    {
                        new()
                        {
                            Id = 1337,
                            Identifier = new Identifier
                            {
                                Value = "searchedFileName1234"
                            }
                        }
                    }
                }
            });

        _ilms.SearchWorldsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new LMSWorldListResponse
        {
            Courses = new List<MoodleCourse>
            {
                new()
                {
                    Id = 1
                }
            }
        });

        _ilms.GetWorldContentAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new[]
        {
            new WorldContent
            {
                Modules = new List<Modules>
                {
                    new()
                    {
                        Name = "searchedFileNamasdasdasde",
                        contextid = 123
                    }
                }
            }
        });

        // Act
        //Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await systemUnderTest.Handle(new GetElementLmsInformationCommand
            {
                WorldId = 1,
                ElementId = 555,
                WebServiceToken = "token"
            }, CancellationToken.None));
    }
}