﻿using AdLerBackend.Application.Common.Exceptions;
using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.InternalUseCases.CheckUserPrivileges;
using AdLerBackend.Application.Common.Responses.Course;
using AdLerBackend.Application.Common.Responses.LMSAdapter;
using AdLerBackend.Application.Course.GetCoursesForAuthor;
using AdLerBackend.Application.Moodle.GetUserData;
using AdLerBackend.Domain.Entities;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

#pragma warning disable CS8618

namespace AdLerBackend.Application.UnitTests.Course.GetCourseForAuthor;

public class GetCourseForAuthorHandlerTest
{
    private ICourseRepository _courseRepository;
    private IMediator _mediator;

    [SetUp]
    public void Setup()
    {
        _courseRepository = Substitute.For<ICourseRepository>();
        _mediator = Substitute.For<IMediator>();
    }

    [Test]
    public async Task Handle_GiveUnauthorotisedUser_ShouldThrow()
    {
        // Arrange
        var request = new GetCoursesForAuthorCommand
        {
            WebServiceToken = "testToken",
            AuthorId = 1
        };

        _mediator.Send(Arg.Any<CheckUserPrivilegesCommand>()).Throws(new ForbiddenAccessException(""));

        var systemUnderTest = new GetCoursesForAuthorHandler(_courseRepository, _mediator);

        // Act

        var exception =
            Assert.ThrowsAsync<ForbiddenAccessException>(async () =>
                await systemUnderTest.Handle(request, CancellationToken.None));


    }

    [Test]
    public async Task Handle_GiveAuthorId_ShouldReturnCourses()
    {
        // Arrange
        var request = new GetCoursesForAuthorCommand
        {
            WebServiceToken = "testToken",
            AuthorId = 1
        };

        // Mock Mediatr Response for GetMoodleUserDataCommand
        var moodleUserData = new MoodleUserDataResponse
        {
            IsAdmin = true,
            UserId = 1,
            MoodleUserName = "userName"
        };
        _mediator.Send(Arg.Any<GetMoodleUserDataCommand>()).Returns(moodleUserData);

        _courseRepository.GetAllCoursesForAuthor(1).Returns(new List<CourseEntity>
        {
            new()
            {
                Id = 1,
                Name = "Test Course",
                AuthorId = 1
            }
        });

        var systemUnderTest = new GetCoursesForAuthorHandler(_courseRepository, _mediator);

        // Act
        var result = await systemUnderTest.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf(typeof(GetCourseOverviewResponse)));
    }
}