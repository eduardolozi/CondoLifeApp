using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BlazorApp.Enums {
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum UserRoleEnum {
		[Description("Morador")]
		Resident,
		[Description("Subsíndico")]
		Submanager,
		[Description("Síndico")]
		Manager,
		[Description("Administrador")]
		Admin
	}
}
