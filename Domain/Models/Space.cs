using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Space {
        public int Id { get; set; }

        public string Name { get; set; }

        [NotMapped]
        public Photo? Photo { get; set; }

        public string? PhotoUrl { get; set; }

        public bool Availability { get; set; }

        public int CondominiumId {  get; set; }

        [JsonIgnore] 
        public Condominium? Condominium { get; set; }

        [JsonIgnore]
        public List<Booking>? Bookings { get; set; }
    }
}
