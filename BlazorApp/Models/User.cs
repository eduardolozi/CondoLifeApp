using BlazorApp.Enums;

namespace BlazorApp.Models {
	public class User {
		public int? Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public UserRoleEnum Role { get; set; }
		public Photo? Photo { get; set; }
		public string? PhotoUrl { get; set; }
		public int? Apartment { get; set; }
		public string? Block { get; set; }
		public int CondominiumId { get; set; }
		public Condominium? Condominium { get; set; }
	}
}
