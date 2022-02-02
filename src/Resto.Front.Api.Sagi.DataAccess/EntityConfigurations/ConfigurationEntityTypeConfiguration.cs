using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.EntityConfigurations
{
	internal class ConfigurationEntityTypeConfiguration : BaseEntityTypeConfiguration<ConfigurationEntity>
	{
		/// <inheritdoc />
		protected override void ConfigureEntity(EntityTypeBuilder<ConfigurationEntity> builder)
		{
			builder.Property(x => x.WebAddress)
				.IsRequired();

			builder.Property(x => x.PhoneNumber)
				.IsRequired();

			builder.Property(x => x.Password)
				.IsRequired();

			builder.HasData(new ConfigurationEntity
			{
				Id = 1,
				WebAddress = "https://test.sagi.kz",
				PhoneNumber = "+77083154319",
				Password = "123456"
			});
		}
	}
}