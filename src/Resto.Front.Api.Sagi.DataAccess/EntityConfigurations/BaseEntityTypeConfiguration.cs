using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resto.Front.Api.Sagi.Entities.Base;

namespace Resto.Front.Api.Sagi.DataAccess.EntityConfigurations
{
	internal abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
		where TEntity : BaseEntity
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<TEntity> builder)
		{
			builder.HasKey(x => x.Id);
			ConfigureEntity(builder);
		}

		protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
	}
}