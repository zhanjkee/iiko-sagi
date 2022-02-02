using System;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Exceptions
{
	[Serializable]
	public class ApiException : Exception
	{
		public ApiException(string message) : base(message)
		{
		}

		public ApiException(int code, string message) : base($"Code: {code} Message: {message}")
		{
		}

		public ApiException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}