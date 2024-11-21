using System.Text.Json.Serialization;

namespace Domain.Enums {
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PostCategoryEnum {
        LostAndFound,
        Marketing,
        GeralAnnouncements,
        Complaints
    }
}
