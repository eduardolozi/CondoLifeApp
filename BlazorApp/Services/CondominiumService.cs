using BlazorApp.Models;

namespace BlazorApp.Services {
	public class CondominiumService(HttpClient httpClient)
	{
		public async Task<List<Condominium>?> GetAll(Address address) {
			var queryParameters = string.Empty;

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

			try {
				var condos = await httpClient.GetFromJsonAsync<List<Condominium>?>(queryParameters);
				return condos;
			}
			catch (Exception) {
				return null;
			}
		}
	}
}
