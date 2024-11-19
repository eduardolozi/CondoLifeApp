using System.Text.Json.Serialization;

namespace BlazorApp.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationTypeEnum
{
    PostLike,
    CommentPost,
    BookingCreated
}