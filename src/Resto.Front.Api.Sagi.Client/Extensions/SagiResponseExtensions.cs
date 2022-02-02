using System;
using Resto.Front.Api.Sagi.Client.Interfaces.Exceptions;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses;

namespace Resto.Front.Api.Sagi.Client.Extensions
{
	internal static class SagiResponseExtensions
	{
		public static int SuccessStatusCode = 0;
		public static int UserNotRegisteredCode = 100;

		internal static bool CustomerNotRegistered(this SagiResponse response)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			return response.Code == UserNotRegisteredCode;
		}

		internal static bool IsSuccessResponse(this SagiResponse response)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			return response.Code == SuccessStatusCode;
		}

		internal static bool GetAwardReturnsError(this SagiResponse response)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			return response.Code != SuccessStatusCode;
		}

		internal static void ThrowApiException(this SagiResponse response)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			throw new ApiException(response.Message);
		}
	}
}