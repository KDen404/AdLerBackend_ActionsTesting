﻿using AdLerBackend.Application.Common.DTOs.Storage;
using AdLerBackend.Application.Common.Exceptions;
using AdLerBackend.Application.Common.Interfaces;
using AdLerBackend.Application.Common.InternalUseCases.CheckUserPrivileges;
using AdLerBackend.Application.Common.Responses.LMSAdapter;
using AdLerBackend.Application.LMS.GetUserData;
using AdLerBackend.Application.World.WorldManagement.DeleteWorld;
using AdLerBackend.Domain.Entities;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

#pragma warning disable CS8618

namespace AdLerBackend.Application.UnitTests.World.WorldManagement.DeleteWorld;

public class DeleteWorldTest
{
    private IFileAccess _fileAccess;
    private IMediator _mediator;
    private IWorldRepository _worldRepository;


    [SetUp]
    public void Setup()
    {
        _worldRepository = Substitute.For<IWorldRepository>();
        _fileAccess = Substitute.For<IFileAccess>();
        _mediator = Substitute.For<IMediator>();
    }

    [Test]
    public async Task Handle_Valid_ShouldCallDeletionOfCourse()
    {
        // Arrange
        var systemUnderTest = new DeleteWorldHandler(_worldRepository, _fileAccess, _mediator);

        var courseMock = new WorldEntity
        {
            Id = 1,
            AuthorId = 1
        };

        _mediator.Send(Arg.Any<GetLMSUserDataCommand>()).Returns(new LMSUserDataResponse
        {
            UserId = 1
        });

        _worldRepository.GetAsync(Arg.Any<int>()).Returns(courseMock);

        _fileAccess.DeleteWorld(Arg.Any<WorldDeleteDto>()).Returns(true);

        // Act
        var result = await systemUnderTest.Handle(new DeleteWorldCommand
        {
            WorldId = 1,
            WebServiceToken = "testToken"
        }, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        // Expect DeleteCourse to be called once
        _fileAccess.Received(1).DeleteWorld(Arg.Any<WorldDeleteDto>());

        // Assert that DeleteAsync was called
        await _worldRepository.Received(1).DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task Handle_UserNotAdmin_ShouldThorwException()
    {
        // Arrange
        var systemUnderTest = new DeleteWorldHandler(_worldRepository, _fileAccess, _mediator);
        _mediator.Send(Arg.Any<CheckUserPrivilegesCommand>()).Throws(new ForbiddenAccessException(""));

        // Act
        // Assert
        Assert.ThrowsAsync<ForbiddenAccessException>(async () => await systemUnderTest.Handle(new DeleteWorldCommand
        {
            WorldId = 1,
            WebServiceToken = "testToken"
        }, CancellationToken.None));
    }

    [Test]
    public async Task Handle_CourseNotExistent_ShouldThorwException()
    {
        // Arrange
        var systemUnderTest = new DeleteWorldHandler(_worldRepository, _fileAccess, _mediator);

        _worldRepository.GetAsync(Arg.Any<int>()).Returns((WorldEntity?) null);

        _fileAccess.DeleteWorld(Arg.Any<WorldDeleteDto>()).Returns(true);

        // Act
        // Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await systemUnderTest.Handle(new DeleteWorldCommand
        {
            WorldId = 1,
            WebServiceToken = "testToken"
        }, CancellationToken.None));
    }

    [Test]
    public async Task Handle_CourseNotFromSameAuthor_ShouldThorwException()
    {
        // Arrange
        var systemUnderTest = new DeleteWorldHandler(_worldRepository, _fileAccess, _mediator);

        var courseMock = new WorldEntity
        {
            Id = 1,
            AuthorId = 1337
        };

        _mediator.Send(Arg.Any<GetLMSUserDataCommand>()).Returns(new LMSUserDataResponse
        {
            UserId = 1
        });

        _worldRepository.GetAsync(Arg.Any<int>()).Returns(courseMock);

        _fileAccess.DeleteWorld(Arg.Any<WorldDeleteDto>()).Returns(true);

        // Act
        // Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await systemUnderTest.Handle(new DeleteWorldCommand
        {
            WorldId = 1,
            WebServiceToken = "testToken"
        }, CancellationToken.None));
    }
}