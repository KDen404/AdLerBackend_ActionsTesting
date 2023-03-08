using AdLerBackend.Application.Common.DTOs;
using AdLerBackend.Application.Common.ElementStrategies.ScoreElementStrategies.ScoreH5PStrategy;
using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.Responses.LMSAdapter;
using AutoBogus;
using Microsoft.Extensions.Configuration;
using NSubstitute;

#pragma warning disable CS8618

namespace AdLerBackend.Application.UnitTests.LearningElements;

public class ScoreH5PLearningElementTest
{
    private ILMS _ilms;
    private ISerialization _serialization;

    [SetUp]
    public void Setup()
    {
        _ilms = Substitute.For<ILMS>();
        _serialization = Substitute.For<ISerialization>();
    }

    [Test]
    public async Task ScoreH5PElement_Valid_CallsWebservices()
    {
        // Arrange

        var inMemorySettings = new Dictionary<string, string>
        {
            {"moodleUrl", "https://whatever.com"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _ilms.GetLMSUserDataAsync(Arg.Any<string>()).Returns(new LMSUserDataResponse
        {
            IsAdmin = false,
            UserEmail = "email",
            UserId = 1,
            LMSUserName = "moodleUserName"
        });

        var fakeXapi = AutoFaker.Generate<RawH5PEvent>();

        _serialization.GetObjectFromJsonString<RawH5PEvent>(Arg.Any<string>()).Returns(fakeXapi);

        _ilms.GetH5PAttemptsAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new H5PAttempts
        {
            usersattempts = new List<Usersattempt>
            {
                new()
                {
                    scored = new Scored
                    {
                        attempts = new List<Attempt>
                        {
                            new()
                            {
                                success = 1
                            }
                        }
                    }
                }
            }
        });


        var systemUnderTest =
            new ScoreH5PElementStrategyHandler(_serialization, _ilms, configuration);

        // Act
        await systemUnderTest.Handle(new ScoreH5PElementStrategyCommand
        {
            ScoreElementParams = new ScoreElementParams
            {
                SerializedXAPIEvent = "xapiEvent"
            },
            Module = new Modules
            {
                contextid = 123,
                Id = 123,
                Instance = 123,
                Name = "name"
            },

            WebServiceToken = "token"
        }, CancellationToken.None);

        // Assert
        await _ilms.Received(1).GetLMSUserDataAsync(Arg.Any<string>());
    }
}