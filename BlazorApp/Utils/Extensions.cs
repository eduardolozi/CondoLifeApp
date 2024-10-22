using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Utils {
	public static class Extensions {
		public static MultipartFormDataContent? ToFormFile(this IBrowserFile browserFile) {
			if (browserFile is null)
				return null;

			try {
				var fileContent = new MultipartFormDataContent();
				using var content = new StreamContent(browserFile.OpenReadStream(browserFile.Size));
				content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(browserFile.ContentType);
				fileContent.Add(content, "Photo", browserFile.Name);
				return fileContent;
			} catch(Exception e) {
				throw new Exception(e.Message, e);
			}
		}
	}
}
