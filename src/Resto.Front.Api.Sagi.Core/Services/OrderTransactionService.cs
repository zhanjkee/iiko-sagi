using System;
using System.Linq;
using Resto.Front.Api.Sagi.Client.Interfaces;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Auth;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Customers;
using Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Customers;
using Resto.Front.Api.Sagi.Core.Interfaces;
using Resto.Front.Api.Sagi.Core.Interfaces.Exceptions;
using Resto.Front.Api.Sagi.Core.Mappers;
using Resto.Front.Api.Sagi.DataAccess.Interfaces;
using Resto.Front.Api.Sagi.Domain.Branches;
using Resto.Front.Api.Sagi.Domain.Orders;
using Configuration = Resto.Front.Api.Sagi.Domain.Configurations.Configuration;

namespace Resto.Front.Api.Sagi.Core.Services
{
	public class OrderTransactionService : IOrderTransactionService
	{
		private readonly Configuration _configuration;
		private readonly ISagiRestClient _sagiRestClient;
		private readonly IUnitOfWork _unitOfWork;
		private long _branchId;
		private bool _disposed;

		public OrderTransactionService(ISagiRestClientFactory sagiClientFactory, IUnitOfWork unitOfWork)
		{
			if (sagiClientFactory == null) throw new ArgumentNullException(nameof(sagiClientFactory));
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

			_configuration = unitOfWork.ConfigurationRepository.GetConfiguration().ToDomain();
			if (_configuration == null) throw new ConfigurationNotInitializedException();

			_sagiRestClient = sagiClientFactory.Create(_configuration.WebAddress);
		}

		/// <inheritdoc />
		public BranchCustomer FindBranchCustomerByCode(string code)
		{
			if (code == null) throw new ArgumentNullException(nameof(code));

			TryLogin();

			var getCustomerByCodeRequest = new GetCustomerByCodeRequest { Code = code };
			var customerByCode = _sagiRestClient.GetCustomerByCode(getCustomerByCodeRequest);

			if (customerByCode == null) throw new CustomerByCodeNotFoundException(code);

			var getBranchRequest = new GetBranchRequest { BranchId = _branchId };
			var branch = _sagiRestClient.GetBranch(getBranchRequest);

			var getAwardRequest = new GetAwardRequest
			{
				BranchId = branch.Id,
				UserId = customerByCode.Id
			};

			// Default result.
			var awardResponse = new AwardResponse
			{
				Id = string.Empty,
				ReceivedStampCount = 0
			};

			var awardResult = _sagiRestClient.GetAward(getAwardRequest);
			if (awardResult != null) awardResponse = awardResult;

			var getCustomerBalanceRequest = new GetBalanceForBranchRequest
			{
				BranchId = branch.Id,
				GroupId = branch.GroupId,
				UserId = customerByCode.Id
			};
			var customerBalance = _sagiRestClient.GetBalanceForBranch(getCustomerBalanceRequest);

			var customer = customerByCode.ToDomain(customerBalance, awardResponse);
			return branch.ToDomain(customer);
		}

		/// <inheritdoc />
		public BranchCustomer FindBranchCustomerByPhoneNumber(string phoneNumber)
		{
			if (phoneNumber == null) throw new ArgumentNullException(nameof(phoneNumber));

			TryLogin();

			var customerByPhoneNumber = _sagiRestClient.GetCustomerByPhoneNumber(new GetCustomerByPhoneNumberRequest
			{
				PhoneNumber = phoneNumber
			});

			var branch = _sagiRestClient.GetBranch(new GetBranchRequest { BranchId = _branchId });

			// Default result.
			var awardResponse = new AwardResponse
			{
				Id = string.Empty,
				ReceivedStampCount = 0
			};

			// If API returns user not registered.
			if (customerByPhoneNumber == null)
			{
				customerByPhoneNumber = new CustomerResponse { Phone = phoneNumber };
			}
			else
			{
				var awardResult = _sagiRestClient.GetAward(new GetAwardRequest
				{
					BranchId = branch.Id,
					UserId = customerByPhoneNumber.Id
				});

				if (awardResult != null) awardResponse = awardResult;
			}

			var customer = customerByPhoneNumber.ToDomain(0, awardResponse);
			return branch.ToDomain(customer);
		}

		/// <inheritdoc />
		public OrderTransaction GetOrderTransactionById(string orderId)
		{
			if (orderId == null) throw new ArgumentNullException(nameof(orderId));

			var orderTransaction = _unitOfWork.OrderTransactionRepository.GetOrderTransactionByIikoOrderId(orderId);
			if (orderTransaction == null)
				return null;

			return orderTransaction.ToDomain();
		}

		/// <inheritdoc />
		public void CreateOrderTransaction(OrderTransaction orderTransaction)
		{
			if (orderTransaction == null) throw new ArgumentNullException(nameof(orderTransaction));

			var orderTransactionEntity =
				_unitOfWork.OrderTransactionRepository.GetOrderTransactionByIikoOrderId(orderTransaction.IikoOrderId);

			if (orderTransactionEntity != null)
			{
				_unitOfWork.OrderTransactionRepository.DeleteOrderTransaction(orderTransactionEntity.Id);
				_unitOfWork.SaveChanges();
			}

			_unitOfWork.OrderTransactionRepository.CreateOrderTransaction(orderTransaction.ToEntity());
			_unitOfWork.SaveChanges();
		}

		/// <inheritdoc />
		public void CommitTransaction(string orderId, string paymentMethod = "CASH")
		{
			if (orderId == null) throw new ArgumentNullException(nameof(orderId));

			var orderTransaction = GetOrderTransactionById(orderId);

			if (orderTransaction == null) throw new OrderNotFoundException(orderId);

			orderTransaction.PaymentMethod = paymentMethod;

			TryLogin();

			var branchCustomer = orderTransaction.Detail.BranchCustomer;

			if (orderTransaction.GiveAward)
			{
				var giveAwardRequest = new GiveAwardRequest
				{
					AwardId = branchCustomer.Customer.AwardId,
					BranchId = branchCustomer.Id,
					Body = new GiveAwardRequestBody
					{
						StampCount = branchCustomer.Customer.ReceivedStampCount,
						UserId = branchCustomer.Customer.Id
					}
				};

				_sagiRestClient.GiveAward(giveAwardRequest);
			}

			var createOrderForSendCashbackRequest = new CreateOrderForSendCashbackRequest
			{
				BranchId = branchCustomer.Id,
				Body = new CreateOrderForSendCashbackRequestBody
				{
					UserId = branchCustomer.Customer.CustomerId,
					Amount = Convert.ToInt32(orderTransaction.Amount),
					Comment = orderTransaction.IikoOrderId,
					UserPhone = branchCustomer.Customer.PhoneNumber,
					AddStamp = orderTransaction.AddStamp,
					StampCount = orderTransaction.StampCount,
					GiveAward = orderTransaction.GiveAward
				}
			};

			var createOrderForSendCashbackResponse =
				_sagiRestClient.CreateOrderForSendCashback(createOrderForSendCashbackRequest);

			var sendMoneyToUserRequest = new SendMoneyToUserRequest
			{
				OrderId = createOrderForSendCashbackResponse.Id,
				Body = new SendMoneyToUserRequestBody
				{
					Amount = orderTransaction.WriteOffSum ?? 0,
					UseBalance = orderTransaction.WriteOffSum.HasValue,
					PaymentMethod = orderTransaction.PaymentMethod
				}
			};

			_sagiRestClient.SendMoneyToUser(sendMoneyToUserRequest);

			orderTransaction.SagiOrderId = sendMoneyToUserRequest.OrderId;

			_unitOfWork.OrderTransactionRepository.UpdateOrderTransaction(orderTransaction.ToEntity());
			_unitOfWork.SaveChanges();
		}

		/// <inheritdoc />
		public void RollbackTransaction(string orderId)
		{
			if (orderId == null) throw new ArgumentNullException(nameof(orderId));

			var orderTransaction = GetOrderTransactionById(orderId);

			if (orderTransaction == null) throw new OrderNotFoundException(orderId);

			TryLogin();

			var revertOrderTransactionRequest = new RevertOrderTransactionRequest
			{
				OrderId = orderTransaction.SagiOrderId
			};

			_sagiRestClient.RevertOrderTransaction(revertOrderTransactionRequest);

			orderTransaction.Detail.WasRefund = true;
			orderTransaction.Detail.RefundDateTimeUtc = DateTime.UtcNow;

			_unitOfWork.OrderTransactionRepository.UpdateOrderTransaction(orderTransaction.ToEntity());
			_unitOfWork.SaveChanges();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void TryLogin()
		{
			var bearerTokenLifetime = _sagiRestClient.GetBearerTokenLifetime();

			if (bearerTokenLifetime.HasValue &&
			    bearerTokenLifetime.Value > DateTime.Now)
				return;

			var loginRequest = new LoginRequest
			{
				PhoneNumber = _configuration.PhoneNumber,
				Password = _configuration.Password
			};

			var loginResponse = _sagiRestClient.Login(loginRequest);

			// Устанавлием токен доступа.
			_sagiRestClient.SetBearerToken(loginResponse.Token);

			// Сохраняем идентификатор филиала.
			_branchId = loginResponse.Organization.Branches.First();
		}

		public virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_unitOfWork?.Dispose();
				_sagiRestClient?.Dispose();
			}

			_disposed = true;
		}
	}
}