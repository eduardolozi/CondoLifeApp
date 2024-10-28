using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Space {
        public int Id { get; set; }

        public string Name { get; set; }

        [NotMapped]
        public Photo? Photo { get; set; }

        public string? PhotoUrl { get; set; }

        public int CondominiumId {  get; set; }

        public bool Disponibilidade { get; set; }

        [JsonIgnore]
        public Condominium Condominium {  get; set; }

        public List<Booking> Bookings { get; set; }
    }
}
