using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches
{
	public class GetBranchResponse
	{
		[JsonProperty("id")] public long Id { get; set; }

		[JsonProperty("name")] public string Name { get; set; }

		[JsonProperty("description")] public string Description { get; set; }

		[JsonProperty("group_id")] public long GroupId { get; set; }

		[JsonProperty("rule")] public Rule Rule { get; set; }

		[JsonProperty("configuration")] public Configuration Configuration { get; set; }
	}
}