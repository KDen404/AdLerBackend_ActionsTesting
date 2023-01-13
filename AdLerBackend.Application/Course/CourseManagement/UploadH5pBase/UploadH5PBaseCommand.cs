﻿using AdLerBackend.Application.Common;
#pragma warning disable CS8618
namespace AdLerBackend.Application.Course.CourseManagement.UploadH5pBase;

public record UploadH5PBaseCommand : CommandWithToken<bool>
{
    public Stream H5PBaseZipStream { get; set; }
}