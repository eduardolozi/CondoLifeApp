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
		
		public static string GetElapsedTime(this DateTime sentDateTime)
		{
			var now = DateTime.Now;
			var timeSpan = now - sentDateTime;

			if (timeSpan.TotalMinutes < 1)
				return "Agora mesmo";

			if (timeSpan.TotalMinutes < 60)
				return $"{Math.Floor(timeSpan.TotalMinutes)} min";

			if (timeSpan.TotalHours < 24)
				return $"{Math.Floor(timeSpan.TotalHours)} h";

			if (timeSpan.TotalDays < 30)
				return $"{Math.Floor(timeSpan.TotalDays)} dias";

			if (timeSpan.TotalDays < 365)
				return $"{Math.Floor(timeSpan.TotalDays / 30)} meses";

			return $"{Math.Floor(timeSpan.TotalDays / 365)} anos";
		}
	}
}
