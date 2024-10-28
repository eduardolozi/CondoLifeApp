using Domain.Enums;

namespace Domain.Models {
    public class Booking {
        public int Id { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public BookingStatusEnum Status { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SpaceId { get; set; }
        public Space Space { get; set; }
    }
}
