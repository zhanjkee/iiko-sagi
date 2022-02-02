using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Resto.Front.Api.Sagi.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WebAddress = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    IikoOrderId = table.Column<string>(nullable: true),
                    SagiOrderId = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    WriteOffSum = table.Column<int>(nullable: true),
                    AddStamp = table.Column<bool>(nullable: false),
                    GiveAward = table.Column<bool>(nullable: false),
                    StampCount = table.Column<int>(nullable: false),
                    PaymentMethod = table.Column<string>(nullable: true),
                    TransactionDetail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTransactions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Id", "Password", "PhoneNumber", "WebAddress" },
                values: new object[] { 1, "123456", "+77083154319", "https://test.sagi.kz" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "OrderTransactions");
        }
    }
}
