using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Like {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        [JsonIgnore] public User User { get; set; }
        [JsonIgnore] public Post Post { get; set; }
    }
}
