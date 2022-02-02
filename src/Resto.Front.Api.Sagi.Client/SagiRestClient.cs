using System;
using Resto.Front.Api.Sagi.Client.Extensions;
using Resto.Front.Api.Sagi.Client.Interfaces;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Auth;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Customers;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Auth;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Customers;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Orders;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;

namespace Resto.Front.Api.Sagi.Client
{
	public class SagiRestClient : ISagiRestClient
	{
		private readonly RestClient _client;
		private DateTime? _bearerTokenLifetime;
		private bool _disposed;

		public SagiRestClient(string webAddress)
		{
			if (webAddress == null) throw new ArgumentNullException(nameof(webAddress));
			_client = new RestClient(webAddress);
			_client.UseSerializer<JsonNetSerializer>();
		}

		/// <inheritdoc />
		public void SetBearerToken(string token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));
			_client.Authenticator = new JwtAuthenticator(token);
			_bearerTokenLifetime = DateTime.Now.AddMinutes(DefaultBearerTokenLifetimeInMinutes);
		}

		/// <inheritdoc />
		public DateTime? GetBearerTokenLifetime()
		{
			return _bearerTokenLifetime;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing) _client?.Dispose();
			_disposed = true;
		}

		#region Auth

		/// <inheritdoc />
		public int DefaultBearerTokenLifetimeInMinutes { get; } = 10;

		/// <inheritdoc />
		public LoginResponse Login(LoginRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest("api/v1/auth/business", Method.Post)
			{
				RequestFormat = DataFormat.Json
			};
			restRequest.AddBody(request);

			var sagiResponse = _client.GetResult<SagiResponse<LoginResponse>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public RefreshTokenResponse RefreshToken(RefreshTokenRequest request)
		{
			var restRequest = new RestRequest("api/v1/auth/refresh_token", Method.Post)
			{
				RequestFormat = DataFormat.Json
			};
			restRequest.AddBody(request);

			var sagiResponse = _client.GetResult<SagiResponse<RefreshTokenResponse>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		#endregion

		#region Branch

		/// <inheritdoc />
		public AwardResponse GetAward(GetAwardRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/branches/{request.BranchId}/customers/{request.UserId}/award");
			var sagiResponse = _client.GetResult<SagiResponse<AwardResponse>>(restRequest);
			if (sagiResponse.GetAwardReturnsError()) return null;
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public void GiveAward(GiveAwardRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/branches/{request.BranchId}/awards/{request.AwardId}/give",
				Method.Post)
			{
				RequestFormat = DataFormat.Json
			};
			restRequest.AddBody(request.Body);

			_client.ExecuteAsync(restRequest).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public GetBranchResponse GetBranch(GetBranchRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/branches/{request.BranchId}");
			var sagiResponse = _client.GetResult<SagiResponse<GetBranchResponse>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public decimal GetBalanceForBranch(GetBalanceForBranchRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest =
				new RestRequest(
					$"api/v1/branches/{request.BranchId}/private/balance?user_id={request.UserId}&group_id={request.GroupId}");
			var sagiResponse = _client.GetResult<SagiResponse<decimal>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public int GetCashbackForUser(GetCashbackForUserRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/branches/{request.BranchId}/cashback?user_id={request.UserId}");
			var sagiResponse = _client.GetResult<SagiResponse<int>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		#endregion

		#region Customer

		/// <inheritdoc />
		public CustomerResponse GetCustomerByCode(GetCustomerByCodeRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/promoters/find?code={request.Code}");
			var sagiResponse = _client.GetResult<SagiResponse<CustomerResponse>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public CustomerResponse GetCustomerByPhoneNumber(GetCustomerByPhoneNumberRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			// NOTE: Replace phone number.
			var phoneNumber = request.PhoneNumber.Replace("+", "");
			var restRequest = new RestRequest($"api/v1/promoters/find?phone={phoneNumber}");
			var sagiResponse = _client.GetResult<SagiResponse<CustomerResponse>>(restRequest);
			if (sagiResponse.CustomerNotRegistered()) return null;
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		#endregion

		#region Order

		/// <inheritdoc />
		public CreateOrderForSendCashbackResponse CreateOrderForSendCashback(CreateOrderForSendCashbackRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/branches/{request.BranchId}/cash/order", Method.Post)
			{
				RequestFormat = DataFormat.Json
			};
			restRequest.AddBody(request.Body);

			var sagiResponse = _client.GetResult<SagiResponse<CreateOrderForSendCashbackResponse>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public SendMoneyToUserResponse SendMoneyToUser(SendMoneyToUserRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/orders/{request.OrderId}/cash/complete", Method.Post)
			{
				RequestFormat = DataFormat.Json
			};
			restRequest.AddBody(request.Body);

			var sagiResponse = _client.GetResult<SagiResponse<SendMoneyToUserResponse>>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
			return sagiResponse.Data;
		}

		/// <inheritdoc />
		public void RevertOrderTransaction(RevertOrderTransactionRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var restRequest = new RestRequest($"api/v1/orders/{request.OrderId}/revert-transaction", Method.Post);

			var sagiResponse = _client.GetResult<SagiResponse>(restRequest);
			if (!sagiResponse.IsSuccessResponse()) sagiResponse.ThrowApiException();
		}

		#endregion
	}
}