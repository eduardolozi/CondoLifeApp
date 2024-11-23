using System.Text.Json.Serialization;

namespace Domain.Models;

public class PostMedias
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int PostId { get; set; }
    [JsonIgnore]
    public Post Post { get; set; }
}