using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.EntityConfigurations
{
	internal class OrderTransactionEntityTypeConfiguration : BaseEntityTypeConfiguration<OrderTransactionEntity>
	{
		/// <inheritdoc />
		protected override void ConfigureEntity(EntityTypeBuilder<OrderTransactionEntity> builder)
		{
		}
	}
}