﻿#pragma warning disable CS8618
namespace AdLerBackend.Application.Common.Responses.Course;

public class GetCourseOverviewResponse
{
    public IList<CourseResponse> Courses { get; set; }
}