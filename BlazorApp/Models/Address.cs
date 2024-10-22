using System.Text.Json.Serialization;

namespace BlazorApp.Models {
	public class Address {
		public int? Id { get; set; } = 0;

		public string? Country { get; set; }

		public string? State { get; set; }

		public string? City { get; set; }

		public string? PostalCode { get; set; }

		public int CondominiumId { get; set; }
	}
}
