﻿#pragma warning disable CS8618
namespace AdLerBackend.Application.Common.DTOs.Storage;

public class WorldDeleteDto
{
    public int AuthorId { get; init; }
    public string WorldName { get; init; }
}