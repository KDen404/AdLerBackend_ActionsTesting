﻿using System.Text.Json.Serialization;

namespace AdLerBackend.Infrastructure.Moodle.ApiResponses;

public class PluginUUIDResponse
{
    [JsonPropertyName("course_id")] public string CourseId { get; set; }

    [JsonPropertyName("element_type")] public string ElementType { get; set; }

    [JsonPropertyName("uuid")] public string Uuid { get; set; }

    [JsonPropertyName("moodle_id")] public int MoodleId { get; set; }

    [JsonPropertyName("context_id")] public int ContextId { get; set; }
}