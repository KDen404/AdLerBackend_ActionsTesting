﻿using JetBrains.Annotations;

namespace AdLerBackend.Domain.Entities;

public class WorldEntity : IBaseEntity
{
    /// <summary>
    ///     This Empty Constructor is needed for EntityFramework as well as for AutoMapper.
    ///     see https://docs.microsoft.com/en-us/ef/core/modeling/constructors
    ///     (EF Core cannot set navigation properties using a constructor.)
    /// </summary>
    [UsedImplicitly]
    internal WorldEntity()
    {
        // Initialize every property with a default value
        Id = null;
        Name = "";
        H5PFilesInCourse = new List<H5PLocationEntity>();
        DslLocation = "";
        AuthorId = 0;
    }


    public WorldEntity(string name, List<H5PLocationEntity> h5PFilesInCourse, string dslLocation, int authorId,
        int? id = null)
    {
        Id = id;
        Name = name;
        H5PFilesInCourse = h5PFilesInCourse;
        DslLocation = dslLocation;
        AuthorId = authorId;
    }

    public string Name { get; set; }
    public List<H5PLocationEntity> H5PFilesInCourse { get; set; }
    public string DslLocation { get; set; }
    public int AuthorId { get; set; }
    public int? Id { get; set; }
}