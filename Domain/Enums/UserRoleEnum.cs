using System.Text.Json.Serialization;

namespace Domain.Enums {
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRoleEnum {
		Resident,
		Counselor,
		Manager,
        Admin
	}
}
