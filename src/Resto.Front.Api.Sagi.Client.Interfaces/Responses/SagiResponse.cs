using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses
{
	public class SagiResponse
	{
		[JsonProperty("code")] public int Code { get; set; }

		[JsonProperty("message")] public string Message { get; set; }
	}

	public class SagiResponse<T> : SagiResponse
	{
		[JsonProperty("data")] public T Data { get; set; }
	}
}