using System;
using Resto.Front.Api.Sagi.Client.Interfaces.Exceptions;
using RestSharp;

#if DEBUG

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Resto.Front.Api.Sagi.Infrastructure.Logging;

#endif

namespace Resto.Front.Api.Sagi.Client.Extensions
{
	internal static class RestClientExtensions
	{
		internal static T GetResult<T>(this RestClient client, RestRequest request)
		{
			if (client == null) throw new ArgumentNullException(nameof(client));
			if (request == null) throw new ArgumentNullException(nameof(request));

#if DEBUG
			request.OnBeforeRequest = OnBeforeRequest;
			request.OnAfterRequest = OnAfterRequest;
#endif

			var response = client.ExecuteAsync<T>(request).GetAwaiter().GetResult();
			if (response.Data == null)
				throw new ApiException($"Sagi API error {response.Content}");

			return response.Data;
		}

#if DEBUG
		private static ValueTask OnBeforeRequest(HttpRequestMessage request)
		{
			var sb = new StringBuilder()
				.AppendLine("Request:")
				.AppendLine(request.ToString())
				.AppendLine(request?.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? "");

			StaticLogger.LogInfo(sb.ToString());

			return new ValueTask();
		}

		private static ValueTask OnAfterRequest(HttpResponseMessage response)
		{
			var sb = new StringBuilder()
				.AppendLine("Response:")
				.AppendLine(response.ToString())
				.AppendLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

			StaticLogger.LogInfo(sb.ToString());

			return new ValueTask();
		}
#endif
	}
}