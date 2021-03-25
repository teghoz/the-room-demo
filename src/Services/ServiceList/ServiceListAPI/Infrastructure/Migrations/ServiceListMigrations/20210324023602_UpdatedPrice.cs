using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceListAPI.Infrastructure.Migrations.ServiceListMigrations
{
    public partial class UpdatedPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceAfterDiscount",
                table: "ServiceLists",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ServiceListItemPromos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsePercentage = table.Column<bool>(type: "bit", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercentage = table.Column<double>(type: "float", nullable: false),
                    ServiceListItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceListItemPromos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceListItemPromos_ServiceLists_ServiceListItemId",
                        column: x => x.ServiceListItemId,
                        principalTable: "ServiceLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceListItemPromos_ServiceListItemId",
                table: "ServiceListItemPromos",
                column: "ServiceListItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceListItemPromos");

            migrationBuilder.DropColumn(
                name: "PriceAfterDiscount",
                table: "ServiceLists");
        }
    }
}
