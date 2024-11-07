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
	}
}
