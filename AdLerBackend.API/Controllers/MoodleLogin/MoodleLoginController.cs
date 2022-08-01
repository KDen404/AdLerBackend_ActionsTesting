﻿using AdLerBackend.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Moodle.Commands;

namespace AdLerBackend.Controllers.MoodleLogin;

[ApiController]
[Route("api/[controller]")]
public class MoodleLoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoodleLoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<MoodleUserDataDTO>> Login([FromBody] MoodleLoginCommand command)
    {
        return await _mediator.Send(command);
    }
}