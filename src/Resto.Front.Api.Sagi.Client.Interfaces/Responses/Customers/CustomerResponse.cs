using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Customers
{
	public class CustomerResponse
	{
		[JsonProperty("id")] public long Id { get; set; }

		[JsonProperty("first_name")] public string FirstName { get; set; }

		[JsonProperty("last_name")] public string LastName { get; set; }

		[JsonProperty("gender")] public string Gender { get; set; }

		[JsonProperty("avatar")] public string Avatar { get; set; }

		[JsonProperty("phone")] public string Phone { get; set; }

		[JsonProperty("username")] public string UserName { get; set; }
	}
}