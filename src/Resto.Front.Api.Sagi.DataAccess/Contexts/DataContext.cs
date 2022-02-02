using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.Contexts
{
	internal class DataContext : DbContext
	{
		public const string SchemaName = "iiko-sagi.db";

		public DataContext()
		{
			const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DatabasePath = Path.Combine(path, SchemaName);
		}

		public string DatabasePath { get; }

		public DbSet<ConfigurationEntity> Configurations { get; set; }
		public DbSet<OrderTransactionEntity> OrderTransactions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite(InitializeSqLiteConnection(DatabasePath));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
		}

		private static SqliteConnection InitializeSqLiteConnection(string dataSource)
		{
			return new SqliteConnection(new SqliteConnectionStringBuilder
			{
				DataSource = dataSource,
				Password = "yzOvbZzHDEWEblCb"
			}.ToString());
		}
	}
}