using BlazorApp.Models;

namespace BlazorApp.Services {
	public class CondominiumService {
		private readonly HttpClient _httpClient;
		public CondominiumService(HttpClient httpClient) {
			_httpClient = httpClient;
		}

		public async Task<List<Condominium>?> GetAll(Address address) {
			var queryParameters = string.Empty;

			if (address != null) {
				if (address.Country != null) {
					queryParameters += $"?Address.Country={address.Country}";
				}
				if (address.State != null) {
					queryParameters += $"&Address.State={address.State}";
				}
				if (address.City != null) {
					queryParameters += $"&Address.City={address.City}";
				}
				if (address.PostalCode != null) {
					queryParameters += $"&Address.PostalCode={address.PostalCode}";
				}
			}

			try {
				var condos = await _httpClient.GetFromJsonAsync<List<Condominium>?>(queryParameters);
				return condos;
			}
			catch (Exception) {
				return null;
			}
		}
	}
}
