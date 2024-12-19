using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
	public class User {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        [NotMapped] public string? Password { get; set; }
        [JsonIgnore] public string? PasswordHash { get; set; } = null!;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsChangePasswordConfirmed { get; set; } = false;
        public UserRoleEnum Role { get; set; }
        public required int Apartment { get; set; }
        public string? Block {  get; set; }
        [NotMapped] public Photo? Photo { get; set; }
        public string? PhotoUrl { get; set; }
        public bool NotifyEmail { get; set; }
        public bool NotifyPhone { get; set; }
        public int? NotificationLifetime { get; set; }
        public int CondominiumId { get; set; }
        [JsonIgnore] public Condominium? Condominium { get; set; }
        [JsonIgnore] public List<UserNotification>? UserNotifications { get; set; }
    }
}
