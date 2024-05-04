using AppleStore.Data;
using Newtonsoft.Json;

namespace AppleStore.Models.Entities.Google
{
	public class TokenGoogle
	{
		//[JsonProperty(PropertyName = "error_description")]
		public string? error_description { get; set; }
		//[JsonProperty(PropertyName = "name")]
		public string? name { get; set; }
		//[JsonProperty(PropertyName = "email")]
		public string? email { get; set; }
	}

	public class LoginGoogle
	{
		private static HttpClient httpClient = new HttpClient();
		private readonly ApplicationDbContext? _context;
        public static string IDClientGoogle = "626942887479-fkj6mb8e0oupps6p7b414tdnfd6uq7kc.apps.googleusercontent.com";

        public static async Task<TokenGoogle> VerifyTokenGoogle(string token)
		{
			TokenGoogle json = new();
			using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={token}"))
			{
				requestMessage.Headers.Add("Accept", "application/json");
				HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
				if (response.IsSuccessStatusCode)
				{
					var t = JsonConvert.DeserializeObject<TokenGoogle>(await response.Content.ReadAsStringAsync());
					if (t?.error_description == null)
					{
						json.name = t.name;
						json.email = t.email;
					}
					else json.error_description = t.error_description;
				}
			}
			return json;
		}
	}
}
