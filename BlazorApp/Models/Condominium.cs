namespace BlazorApp.Models {
	public class Condominium {
		public int Id { get; set; } = 0;

		public string? Name { get; set; }

		public Address Address { get; set; } = null!;
	}
}
