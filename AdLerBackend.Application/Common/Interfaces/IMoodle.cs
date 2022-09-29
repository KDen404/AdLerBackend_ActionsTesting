﻿using AdLerBackend.Application.Common.Responses.LMSAdapter;

namespace AdLerBackend.Application.Common.Interfaces;

public interface IMoodle
{
    /// <summary>
    ///     Gets the Moodle User Data for a given Webserice Token
    /// </summary>
    /// <param name="token">Moodle Webservice Token</param>
    /// <returns>Moodle User Data6</returns>
    Task<MoodleUserDataResponse> GetMoodleUserDataAsync(string token);

    /// <summary>
    ///     Gets the Moodle Webservice Token for a given Account
    /// </summary>
    /// <param name="userName">Moodle User Name</param>
    /// <param name="password">Moodle User Password</param>
    /// <returns>The Moodle Token</returns>
    Task<MoodleUserTokenResponse> GetMoodleUserTokenAsync(string userName, string password);

    /// <summary>
    ///     Searches all Courses, that are avalibale for the given Moodle User
    /// </summary>
    /// <param name="token">Token of the Moodle User</param>
    /// <param name="searchString">The Course to get Searched for</param>
    /// <returns>A List of all found Coruses</returns>
    Task<MoodleCourseListResponse> SearchCoursesAsync(string token, string searchString, bool limitToEnrolled = false);

    /// <summary>
    ///     Gets the Contents of a Course
    /// </summary>
    /// <param name="token">Token of the Moodle User</param>
    /// <param name="courseId">ID of the Course</param>
    /// <returns>All User-Visible Contents of a Course as Array</returns>
    Task<CourseContent[]> GetCourseContentAsync(string token, int courseId);

    /// <summary>
    ///     Gets all Courses that the User is enrolled in
    /// </summary>
    /// <param name="token">The Users Webservice Token</param>
    /// <returns></returns>
    Task<MoodleCourseListResponse> GetCoursesForUserAsync(string token);

    /// <summary>
    ///     Determines whether the given User is an admin in the Moodle System
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> IsMoodleAdminAsync(string token);

    /// <summary>
    ///     Processes an XAPI Statement
    /// </summary>
    /// <param name="token"></param>
    /// <param name="statement"></param>
    /// <returns>Returns True, if the Statement hase been processed successfully </returns>
    Task<bool> ProcessXAPIStatementAsync(string token, string statement);

    Task<H5PAttempts> GetH5PAttemptsAsync(string token, int h5pActivityId);

    Task<bool> ScoreGenericLearningElement(string token, int learningElementId);
}