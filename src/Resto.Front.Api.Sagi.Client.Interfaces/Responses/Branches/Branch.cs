using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches
{
	public class Branch
	{
		[JsonProperty("id")] public long Id { get; set; }

		[JsonProperty("name")] public string Name { get; set; }

		[JsonProperty("description")] public string Description { get; set; }

		[JsonProperty("group_id")] public long GroupId { get; set; }
	}
}