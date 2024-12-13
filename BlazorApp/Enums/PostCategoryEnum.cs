using System.Text.Json.Serialization;

namespace BlazorApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PostCategoryEnum {
    LostAndFound,
    Marketing,
    GeralAnnouncements,
    Complaints
}