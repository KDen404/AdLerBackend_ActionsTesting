﻿#pragma warning disable CS8618
namespace AdLerBackend.Application.Common.Responses.LMSAdapter;

public class MoodleUserDataResponse
{
    public string MoodleUserName { get; set; }
    public bool IsAdmin { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; }
}