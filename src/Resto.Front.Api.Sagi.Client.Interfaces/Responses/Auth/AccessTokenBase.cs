using System;
using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Auth
{
	public abstract class AccessTokenBase
	{
		private readonly DateTime _systemDateTime;

		protected AccessTokenBase()
		{
			_systemDateTime = DateTime.Now;
		}

		[JsonProperty("access_token")] public string Token { get; set; }

		[JsonProperty("expire")] public DateTime ExpireIn { get; set; }

		public DateTime GetSystemDateTime()
		{
			return _systemDateTime;
		}
	}
}