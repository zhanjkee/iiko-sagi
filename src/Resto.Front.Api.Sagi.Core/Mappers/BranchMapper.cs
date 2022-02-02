using System;
using Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches;
using Resto.Front.Api.Sagi.Domain.Branches;
using Resto.Front.Api.Sagi.Domain.Cashbacks;
using Resto.Front.Api.Sagi.Domain.Customers;
using Award = Resto.Front.Api.Sagi.Domain.Awards.Award;

namespace Resto.Front.Api.Sagi.Core.Mappers
{
	public static class BranchMapper
	{
		public static BranchCustomer ToDomain(this GetBranchResponse source, Customer customer)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return new BranchCustomer
			{
				Id = source.Id,
				GroupId = source.GroupId,
				Customer = customer,
				Award = source.Configuration?.Award.ToDomain(),
				Cashback = source.Rule.ToDomain()
			};
		}

		public static Award ToDomain(this Client.Interfaces.Responses.Branches.Award source)
		{
			if (source == null)
				return new Award
				{
					Enabled = false,
					StampCount = 0
				};

			return new Award
			{
				Enabled = source.Enabled,
				StampCount = source.StampCount
			};
		}

		public static Cashback ToDomain(this Rule source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return new Cashback
			{
				Enabled = true,
				CashbackPercentage = source.CashbackPercentage
			};
		}
	}
}