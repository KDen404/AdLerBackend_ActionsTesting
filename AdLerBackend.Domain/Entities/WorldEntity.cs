﻿#pragma warning disable CS8618
using AdLerBackend.Domain.Entities.Common;

namespace AdLerBackend.Domain.Entities;

public class WorldEntity : BaseEntity
{
    public string Name { get; set; }
    public List<H5PLocationEntity> H5PFilesInCourse { get; set; }
    public string DslLocation { get; set; }
    public int AuthorId { get; set; }
}