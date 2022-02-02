using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Auth
{
	public class LoginRequest
	{
		[JsonProperty("phone")] public string PhoneNumber { get; set; }

		[JsonProperty("password")] public string Password { get; set; }
	}
}