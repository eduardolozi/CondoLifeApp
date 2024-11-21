using System.ComponentModel;
using BlazorApp.Models;

namespace BlazorApp.Utils {
	public static class Extensions {
		public static async Task HandleResponseError(this HttpResponseMessage response)
		{
			var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
			if (problemDetails != null)
				throw new ApplicationException(problemDetails.Detail);
			else
				response.EnsureSuccessStatusCode();
		}

		public static string? GetEnumDescription<T>(this T enumValue)
		{
			if (!typeof(T).IsEnum || enumValue == null)
				return null;

			var description = enumValue.ToString();
			var fieldInfo = enumValue.GetType().GetField(enumValue.ToString()!);

			if (fieldInfo != null)
			{
				var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (attrs != null && attrs.Length > 0)
				{
					description = ((DescriptionAttribute)attrs[0]).Description;
				}
			}

			return description;
		}
	}
}
