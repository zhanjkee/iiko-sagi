using System;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Customers;
using Resto.Front.Api.Sagi.Domain.Customers;

namespace Resto.Front.Api.Sagi.Core.Mappers
{
	public static class CustomerMapper
	{
		public static Customer ToDomain(this CustomerResponse source, decimal balance, AwardResponse award)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (award == null) throw new ArgumentNullException(nameof(award));

			return source.ToDomain(balance, award.Id, award.ReceivedStampCount);
		}

		public static Customer ToDomain(this CustomerResponse source, decimal balance, string awardId, int stampCount)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return new Customer
			{
				CustomerId = source.Id,
				PhoneNumber = source.Phone,
				Balance = balance,
				AwardId = awardId,
				ReceivedStampCount = stampCount
			};
		}
	}
}