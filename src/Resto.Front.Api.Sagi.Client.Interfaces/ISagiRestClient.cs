using System;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Auth;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Customers;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Auth;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Customers;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Orders;

namespace Resto.Front.Api.Sagi.Client.Interfaces
{
	/// <summary>
	///     RestAPI клиент для Sagi.
	/// </summary>
	public interface ISagiRestClient : IDisposable
	{
		int DefaultBearerTokenLifetimeInMinutes { get; }

		/// <summary>
		///     Получить токен.
		/// </summary>
		LoginResponse Login(LoginRequest request);

		/// <summary>
		///     Обновить устаревший токен.
		/// </summary>
		RefreshTokenResponse RefreshToken(RefreshTokenRequest request);

		/// <summary>
		///     Получить информацию о нагарадах.
		/// </summary>
		AwardResponse GetAward(GetAwardRequest request);

		void GiveAward(GiveAwardRequest request);

		/// <summary>
		///     Получить полную информацию о филиале.
		/// </summary>
		GetBranchResponse GetBranch(GetBranchRequest request);

		/// <summary>
		///     Получить доступные бонусы.
		/// </summary>
		decimal GetBalanceForBranch(GetBalanceForBranchRequest request);

		/// <summary>
		///     Получить процент кэшбека.
		/// </summary>
		int GetCashbackForUser(GetCashbackForUserRequest request);

		/// <summary>
		///     Поиск клиента по 6-значному коду.
		/// </summary>
		CustomerResponse GetCustomerByCode(GetCustomerByCodeRequest request);

		/// <summary>
		///     Поиск клиента по номеру телефона.
		/// </summary>
		CustomerResponse GetCustomerByPhoneNumber(GetCustomerByPhoneNumberRequest request);

		/// <summary>
		///     Создать транзакцию для начисления бонуса.
		/// </summary>
		CreateOrderForSendCashbackResponse CreateOrderForSendCashback(CreateOrderForSendCashbackRequest request);

		/// <summary>
		///     Начислить бонусы клиенту.
		/// </summary>
		SendMoneyToUserResponse SendMoneyToUser(SendMoneyToUserRequest request);

		/// <summary>
		///     Откат транзакции.
		/// </summary>
		void RevertOrderTransaction(RevertOrderTransactionRequest request);

		/// <summary>
		///     Задать токен доступа.
		/// </summary>
		void SetBearerToken(string token);

		/// <summary>
		///     Получить время жизни токена.
		/// </summary>
		DateTime? GetBearerTokenLifetime();
	}
}