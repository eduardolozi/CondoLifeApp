using System.Text.Json.Serialization;

namespace Domain.Enums {
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationTypeEnum {
        PostLike,
        CommentPost,
        BookingCreated,
        BookingApproved,
        VotingCreated
    }
}
