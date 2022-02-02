using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Xml.Linq;
using Resto.Front.Api.Attributes.JetBrains;
using Resto.Front.Api.Data.Cheques;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.Data.Organization;
using Resto.Front.Api.Data.Payments;
using Resto.Front.Api.Data.Security;
using Resto.Front.Api.Data.View;
using Resto.Front.Api.Exceptions;
using Resto.Front.Api.Extensions;
using Resto.Front.Api.Sagi.Bootstrapper;
using Resto.Front.Api.Sagi.Client.Interfaces.Exceptions;
using Resto.Front.Api.Sagi.Core.Interfaces;
using Resto.Front.Api.Sagi.Core.Interfaces.Exceptions;
using Resto.Front.Api.Sagi.Domain.Branches;
using Resto.Front.Api.Sagi.Domain.Orders;
using Resto.Front.Api.Sagi.Plugin.Constants;
using Resto.Front.Api.Sagi.Plugin.Extensions;
using Resto.Front.Api.UI;

namespace Resto.Front.Api.Sagi.Plugin
{
	internal sealed class ExternalPaymentProcessor : MarshalByRefObject, IPaymentProcessor, IDisposable
	{
		private const int NotificationTimeoutInSeconds = 30;
		private readonly IOrderTransactionService _orderTransactionService;
		private readonly CompositeDisposable _subscriptions;

		public ExternalPaymentProcessor()
		{
			PluginContext.Log.Info("[ExternalPaymentProcessor]");
			PluginContext.Log.Info("Подписка метода CheckOrder на событие CashChequePrinting");

			_subscriptions = new CompositeDisposable
			{
				PluginContext.Notifications.CashChequePrinting.Subscribe(CheckOrder)
			};

			_orderTransactionService = DependecyInjection.GetService<IOrderTransactionService>();
			if (_orderTransactionService == null)
				throw new PaymentActionFailedException("Cannot resolve sagi order transaction service");
		}

		#region Disposing

		public void Dispose()
		{
			PluginContext.Log.Info("Sagi payment processor disposing...");
			_subscriptions?.Dispose();
			_orderTransactionService?.Dispose();
			PluginContext.Log.Info("Sagi payment processor disposed");
		}

		#endregion

		public string PaymentSystemKey => Resources.SystemKey;
		public string PaymentSystemName => Resources.SystemName;

		public void CollectData(
			Guid orderId,
			Guid paymentTypeId,
			[NotNull] IUser cashier,
			IReceiptPrinter printer,
			IViewManager viewManager,
			IPaymentDataContext context)
		{
			PluginContext.Log.Info("[CollectData]");

			var order = PluginContext.Operations.GetOrderById(orderId);

			if (order == null) throw new PaymentActionFailedException(Resources.TitlePaymentForEmptyOrderNotPossible);

			// Идентификация клиента.
			var inputDialogResult = viewManager.ShowExtendedInputDialog(Resources.TitleCustomerIdentification,
				string.Empty,
				new ExtendedInputDialogSettings
				{
					EnablePhone = true,
					EnableNumericString = true,
					TabTitleNumericString = Resources.TitleCodeInput,
					TabTitlePhone = Resources.TitlePhoneInput
				});

			var inputData = string.Empty;
			var searchByPhone = false;

			switch (inputDialogResult)
			{
				case null:
					throw new PaymentActionFailedException(Resources.TitleUserRefusedInput);
				case StringInputDialogResult inputDialog:
					inputData = inputDialog.Result;
					break;
				case PhoneInputDialogResult inputDialog:
					inputData = inputDialog.PhoneNumber;
					searchByPhone = true;
					break;
			}

			if (string.IsNullOrEmpty(inputData))
				throw new PaymentActionFailedException(Resources.TitleCodeOrNumberWasNotEntered);

			PluginContext.Log.Info(Resources.TitleCustomerIdentification);
			PluginContext.Log.Info(inputData);

			viewManager.ChangeProgressBarMessage(Resources.TitleDataProcessingPleaseWaiting);

			// Поиск клиента Sagi.
			var branchCustomer = FindBranchCustomer(inputData, searchByPhone);

			if (branchCustomer == null) throw new PaymentActionFailedException(Resources.TitleCustomerNotFound);

			var customerFoundMessage = string.Format(
				Resources.TitleCustomerFound,
				branchCustomer.Customer.Balance,
				branchCustomer.Customer.ReceivedStampCount,
				branchCustomer.Award.StampCount);

			PluginContext.Log.Info(customerFoundMessage);
			PluginContext.Operations.AddNotificationMessage(customerFoundMessage, PaymentSystemName,
				TimeSpan.FromSeconds(NotificationTimeoutInSeconds));

			var orderTransaction = new OrderTransaction
			{
				IikoOrderId = orderId.ToString(),
				Amount = order.ResultSum,
				Detail = new OrderTransactionDetail
				{
					BranchCustomer = branchCustomer
				}
			};

			// Сбор данных по списанию/начислению бонусов.
			if (searchByPhone)
				CollectAccrualBonusesData(viewManager, orderTransaction);
			else
				CollectWriteOffBonusesData(viewManager, orderTransaction);

			// Создаем транзакцию в БД.
			_orderTransactionService.CreateOrderTransaction(orderTransaction);

			PluginContext.Operations.AddNotificationMessage(Resources.TitleTransactionDataSaved, PaymentSystemName,
				TimeSpan.FromSeconds(NotificationTimeoutInSeconds));
		}

		public void OnPaymentAdded(
			[NotNull] IOrder order,
			[NotNull] IPaymentItem paymentItem,
			[NotNull] IUser cashier,
			[NotNull] IOperationService operations,
			IReceiptPrinter printer,
			[NotNull] IViewManager viewManager,
			IPaymentDataContext context)
		{
			PluginContext.Log.Info("[OnPaymentAdded]");

			var orderTransaction = _orderTransactionService.GetOrderTransactionById(order.Id.ToString());

			if (orderTransaction == null)
				throw new PaymentActionFailedException(Resources.TitleCannotGetUserDataByOrderId);

			if (orderTransaction.WriteOffSum.HasValue)
				operations.ChangePaymentItemSum(orderTransaction.WriteOffSum.Value, 0m, order.ResultSum, paymentItem,
					order, operations.GetCredentials());
		}

		public void Pay(
			decimal sum,
			[NotNull] IOrder order,
			[NotNull] IPaymentItem paymentItem,
			Guid transactionId,
			[NotNull] IPointOfSale pointOfSale,
			[NotNull] IUser cashier,
			[NotNull] IOperationService operationService,
			IReceiptPrinter printer,
			[NotNull] IViewManager viewManager,
			IPaymentDataContext context)
		{
			PluginContext.Log.Info("[Pay]");

			viewManager.ChangeProgressBarMessage(Resources.TitleTransactionProcessingPleaseWaiting);

			var orderTransaction = _orderTransactionService.GetOrderTransactionById(order.Id.ToString());

			if (orderTransaction == null)
				throw new PaymentActionFailedException(Resources.TitleCannotGetUserDataByOrderId);

			if (!orderTransaction.WriteOffSum.HasValue) return;

			// Списываем бонусы.
			CommitOrderTransaction(orderTransaction, order);
			viewManager.ChangeProgressBarMessage(Resources.TitlePrintSlipReceipt);
			PrintReceiptSlip(printer, orderTransaction.WriteOffSum.Value, order, transactionId);
		}

		public void EmergencyCancelPayment(
			decimal sum,
			Guid? orderId,
			Guid paymentTypeId,
			Guid transactionId,
			[NotNull] IPointOfSale pointOfSale,
			[NotNull] IUser cashier,
			IReceiptPrinter printer,
			IViewManager viewManager,
			IPaymentDataContext context)
		{
			PluginContext.Log.Info("[EmergencyCancelPayment]");

			if (!orderId.HasValue)
				return;

			RevertOrderTransaction(orderId.ToString(), viewManager);
		}

		public void ReturnPayment(
			decimal sum,
			Guid? orderId,
			Guid paymentTypeId,
			Guid transactionId,
			[NotNull] IPointOfSale pointOfSale,
			[NotNull] IUser cashier,
			IReceiptPrinter printer,
			[NotNull] IViewManager viewManager,
			IPaymentDataContext context)
		{
			PluginContext.Log.Info("[ReturnPayment]");

			if (!orderId.HasValue)
				return;

			RevertOrderTransaction(orderId.ToString(), viewManager);
		}

		#region Subscriptions

		private CashCheque CheckOrder(Guid orderId)
		{
			PluginContext.Log.Info("[CheckOrder]");
			try
			{
				var order = PluginContext.Operations.GetOrderById(orderId);

				var orderTransaction = _orderTransactionService.GetOrderTransactionById(order.Id.ToString());

				if (orderTransaction == null)
					throw new PaymentActionFailedException(Resources.TitleCannotGetUserDataByOrderId);

				if (orderTransaction.WriteOffSum.HasValue) return null;

				// Начисляем бонусы.
				CommitOrderTransaction(orderTransaction, order);
				return null;
			}
			catch (ApiException e)
			{
				var message = string.Format(Resources.TitleSagiSideError, e.Message);
				PluginContext.Log.Error(message);
				throw new PaymentActionFailedException(message);
			}
		}

		#endregion

		#region Extensions methods

		/// <summary>
		///     Поиск клиента SAGI.
		/// </summary>
		private BranchCustomer FindBranchCustomer(string inputData, bool searchByPhone)
		{
			PluginContext.Log.Info("[FindBranchCustomer]");
			try
			{
				if (searchByPhone)
				{
					var message = string.Format(Resources.TitleSearchCustomerByPhone, inputData);

					PluginContext.Log.Info(message);
					PluginContext.Operations.AddNotificationMessage(message, PaymentSystemName,
						TimeSpan.FromSeconds(NotificationTimeoutInSeconds));

					// Поиск по номеру телефона.
					return _orderTransactionService.FindBranchCustomerByPhoneNumber(inputData);
				}
				else
				{
					var message = string.Format(Resources.TitleSearchCustomerByCode, inputData);

					PluginContext.Log.Info(message);
					PluginContext.Operations.AddNotificationMessage(message, PaymentSystemName,
						TimeSpan.FromSeconds(NotificationTimeoutInSeconds));

					// Поиск по 6-значному коду.
					return _orderTransactionService.FindBranchCustomerByCode(inputData.ToCodeString());
				}
			}
			catch (CustomerByCodeNotFoundException e)
			{
				throw new PaymentActionFailedException(e.Message);
			}
			catch (ConfigurationNotInitializedException e)
			{
				throw new PaymentActionFailedException(e.Message);
			}
			catch (ApiException e)
			{
				var message = string.Format(Resources.TitleSagiSideError, e.Message);
				PluginContext.Log.Error(message);
				throw new PaymentActionFailedException(message);
			}
		}

		/// <summary>
		///     Сбор данных для начисление бонусов.
		/// </summary>
		private static void CollectAccrualBonusesData(IViewManager viewManager, OrderTransaction orderTransaction)
		{
			PluginContext.Log.Info("[CollectAccrualBonusesData]");

			var needToAccrue = viewManager.ShowYesNoPopup(
				Resources.TitleAccrualBonuses,
				string.Format(Resources.TitleAccrualBonusesInfoMessage, orderTransaction.GetAccrualAmount(),
					orderTransaction.Detail.BranchCustomer.Cashback.CashbackPercentage));

			if (!needToAccrue) throw new PaymentActionFailedException(Resources.TitleCustomerRefusedAccrueBonuses);

			// Сбор данных по наградам и подаркам.
			if (orderTransaction.Detail.BranchCustomer.Award.Enabled)
				CollectAwardOrGiftData(viewManager, orderTransaction);
		}

		/// <summary>
		///     Сбор данных для списания бонусов.
		/// </summary>
		private static void CollectWriteOffBonusesData(IViewManager viewManager, OrderTransaction orderTransaction)
		{
			PluginContext.Log.Info("[CollectWriteOffBonusesData]");

			var branchCustomer = orderTransaction.Detail.BranchCustomer;

			var writeOffDialogResult = viewManager.ShowInputDialog(
				string.Format(Resources.TitleWriteOffBonusesInfoMessage, branchCustomer.Customer.Balance),
				InputDialogTypes.Number, 0);

			// В случае, если клиент отказался от списания, собираем данные для начисление бонуса.
			if (writeOffDialogResult is NumberInputDialogResult writeOff)
			{
				var writeOffSum = writeOff.Number;
				if (writeOffSum > branchCustomer.Customer.Balance)
					throw new PaymentActionFailedException(Resources.TitleNotEnoughBonusesToWriteOff);

				orderTransaction.WriteOffSum = writeOffSum;

				// Cбор данных по подаркам и наградам.
				if (branchCustomer.Award.Enabled) CollectAwardOrGiftData(viewManager, orderTransaction);
			}
			else
			{
				CollectAccrualBonusesData(viewManager, orderTransaction);
			}
		}

		/// <summary>
		///     Сбор данных по наградам и подаркам.
		/// </summary>
		private static void CollectAwardOrGiftData(IViewManager viewManager, OrderTransaction orderTransaction)
		{
			PluginContext.Log.Info("[CollectAwardOrGiftData]");

			var branchCustomer = orderTransaction.Detail.BranchCustomer;
			if (orderTransaction.CanGiveAward())
			{
				orderTransaction.GiveAward = viewManager.ShowYesNoPopup(
					Resources.TitleGiveAward,
					string.Format(Resources.TitleGiveAwardInfoMessage, branchCustomer.Customer.ReceivedStampCount,
						branchCustomer.Award.StampCount));
			}
			else
			{
				orderTransaction.AddStamp = viewManager.ShowYesNoPopup(
					Resources.TitleAddStamp,
					string.Format(Resources.TitleAddStampInfoMessage, branchCustomer.Customer.ReceivedStampCount,
						branchCustomer.Award.StampCount));

				if (!orderTransaction.AddStamp) return;
				var stampCountDialogResult = viewManager.ShowInputDialog(
					string.Format(Resources.TitleStampCountInfoMessage,
						branchCustomer.Customer.ReceivedStampCount,
						orderTransaction.Detail.BranchCustomer.Award.StampCount), InputDialogTypes.Number, 0);

				if (!(stampCountDialogResult is NumberInputDialogResult stampCountResult)) return;
				var stampCount = stampCountResult.Number;
				var receivedStampCount = branchCustomer.Customer.ReceivedStampCount;
				var allowableNumberOfStamps =
					orderTransaction.Detail.BranchCustomer.Award.StampCount - receivedStampCount;

				if (stampCount > allowableNumberOfStamps)
					throw new PaymentActionFailedException(string.Format(Resources.TitleStampFailedMessage,
						allowableNumberOfStamps));

				orderTransaction.StampCount = stampCount;
			}
		}

		/// <summary>
		///     Печать чека.
		/// </summary>
		private void PrintReceiptSlip(IReceiptPrinter printer, int writeOffSum, IOrder order, Guid transactionId)
		{
			PluginContext.Log.Info("[PrintReceiptSlip]");

			var slip = new ReceiptSlip
			{
				Doc =
					new XElement(Tags.Doc,
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleSystem),
							new XAttribute(Data.Cheques.Attributes.Right, PaymentSystemKey),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)),
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleTransactionId),
							new XAttribute(Data.Cheques.Attributes.Right, transactionId.ToString()),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)),
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleDate),
							new XAttribute(Data.Cheques.Attributes.Right, DateTime.Now.ToString(Consts.DateTimeFormat)),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)),
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleOrderId),
							new XAttribute(Data.Cheques.Attributes.Right, order.Number.ToString()),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)),
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleOrderAmount),
							new XAttribute(Data.Cheques.Attributes.Right,
								order.FullSum.ToString(CultureInfo.InvariantCulture)),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)),
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleAmountToPay),
							new XAttribute(Data.Cheques.Attributes.Right,
								order.ResultSum.ToString(CultureInfo.InvariantCulture)),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)),
						new XElement(Tags.Pair,
							new XAttribute(Data.Cheques.Attributes.Left, Resources.TitleWriteOffBonuses),
							new XAttribute(Data.Cheques.Attributes.Right, writeOffSum.ToString()),
							new XAttribute(Data.Cheques.Attributes.Fit, Data.Cheques.Attributes.Right)))
			};

			printer.Print(slip);
		}

		/// <summary>
		///     Совершить транзакцию.
		/// </summary>
		public void CommitOrderTransaction(OrderTransaction orderTransaction, IOrder order)
		{
			PluginContext.Log.Info("[CommitOrderTransaction]");

			if (orderTransaction == null) throw new ArgumentNullException(nameof(orderTransaction));

			if (order == null) throw new ArgumentNullException(nameof(order));

			try
			{
				_orderTransactionService.CommitTransaction(orderTransaction.IikoOrderId, GetPaymentMethod(order));
			}
			catch (ApiException e)
			{
				var message = string.Format(Resources.TitleSagiSideError, e.Message);
				PluginContext.Log.Error(message);
				throw new PaymentActionFailedException(message);
			}

			var notificationMessage = orderTransaction.WriteOffSum.HasValue
				? Resources.TitleWriteOffBonusesSucceeded
				: Resources.TitleAccrualBonusesSucceeded;

			PluginContext.Operations.AddNotificationMessage(notificationMessage, PaymentSystemName,
				TimeSpan.FromSeconds(NotificationTimeoutInSeconds));
		}

		/// <summary>
		///     Откат транзакции.
		/// </summary>
		public void RevertOrderTransaction(string orderId, IViewManager viewManager)
		{
			PluginContext.Log.Info("[RevertOrderTransaction]");

			var orderTransaction = _orderTransactionService.GetOrderTransactionById(orderId);

			if (orderTransaction == null)
				throw new PaymentActionFailedException(Resources.TitleCannotGetUserDataByOrderId);

			if (orderTransaction.Detail.WasRefund) return;

			viewManager.ChangeProgressBarMessage(Resources.TitleCancelTransactionPleaseWaiting);

			try
			{
				_orderTransactionService.RollbackTransaction(orderId);
			}
			catch (ApiException e)
			{
				var message = string.Format(Resources.TitleSagiSideError, e.Message);
				PluginContext.Log.Error(message);
				throw new PaymentActionFailedException(message);
			}
		}

		public string GetPaymentMethod(IOrder order)
		{
			if (order == null) throw new ArgumentNullException(nameof(order));

			foreach (var payment in order.Payments)
				if (payment.Type.Kind == PaymentTypeKind.Card ||
				    payment.Type.Kind == PaymentTypeKind.Cash)
					return payment.Type.Name;

			return Consts.Cash;
		}

		#endregion

		#region Not implemented methods

		public bool OnPreliminaryPaymentEditing(
			[NotNull] IOrder order,
			[NotNull] IPaymentItem paymentItem,
			[NotNull] IUser cashier,
			[NotNull] IOperationService operationService,
			IReceiptPrinter printer,
			[NotNull] IViewManager viewManager,
			IPaymentDataContext context)
		{
			return true;
		}

		public bool CanPaySilently(
			decimal sum,
			Guid? orderId,
			Guid paymentTypeId,
			IPaymentDataContext context)
		{
			return false;
		}

		public void EmergencyCancelPaymentSilently(
			decimal sum,
			Guid? orderId,
			Guid paymentTypeId,
			Guid transactionId,
			[NotNull] IPointOfSale pointOfSale,
			[NotNull] IUser cashier,
			IReceiptPrinter printer,
			IPaymentDataContext context)
		{
		}

		public void ReturnPaymentWithoutOrder(
			decimal sum,
			Guid? orderId,
			Guid paymentTypeId,
			[NotNull] IPointOfSale pointOfSale,
			[NotNull] IUser cashier,
			IReceiptPrinter printer,
			[NotNull] IViewManager viewManager)
		{
		}

		public void PaySilently(
			decimal sum,
			[NotNull] IOrder order,
			[NotNull] IPaymentItem paymentItem,
			Guid transactionId,
			[NotNull] IPointOfSale pointOfSale,
			[NotNull] IUser cashier,
			IReceiptPrinter printer,
			IPaymentDataContext context)
		{
		}

		#endregion
	}
}