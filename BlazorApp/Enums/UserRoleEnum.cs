using System.Text.Json.Serialization;

namespace BlazorApp.Enums {
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum UserRoleEnum {
		Admin,
		Resident,
		Counselor,
		Manager
	}
}
