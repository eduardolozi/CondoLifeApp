using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Models {
    public class Booking {
        public int Id { get; set; }
        
        public DateTime InitialDate { get; set; }
        
        public DateTime FinalDate { get; set; }
        
        public BookingStatusEnum Status { get; set; }
        
        public string? CancellationReason { get; set; }
        
        public string? Description { get; set; }
        
        public int UserId { get; set; }
        
        [NotMapped]
        public string? Username { get; set; }
        
        [JsonIgnore]
        public User? User { get; set; }
        
        public int SpaceId { get; set; }
        
        [JsonIgnore]
        public Space? Space { get; set; }
        
        [NotMapped]
        public string? SpaceName { get; set; }
    }
}
