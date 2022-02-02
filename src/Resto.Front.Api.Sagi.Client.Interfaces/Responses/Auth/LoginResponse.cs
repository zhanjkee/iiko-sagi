using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Auth
{
	public class LoginResponse : AccessTokenBase
	{
		[JsonProperty("muted")] public bool Muted { get; set; }

		[JsonProperty("organization")] public Organization Organization { get; set; }
	}
}