using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNSHOP.Migrations
{
    /// <inheritdoc />
    public partial class AddSubOrderIdToRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Customers_CustomerId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Ratings");

            migrationBuilder.AddColumn<int>(
                name: "SubOrderId",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0);

           

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$g1pkbbyluygBtZd0kXhLj.2uHw9FnPRdvbcYz.Cbzhcn8wMhVtg06");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_SubOrderId",
                table: "Ratings",
                column: "SubOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Customers_CustomerId",
                table: "Ratings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_SubOrders_SubOrderId",
                table: "Ratings",
                column: "SubOrderId",
                principalTable: "SubOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Customers_CustomerId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_SubOrders_SubOrderId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_SubOrderId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "SubOrderId",
                table: "Ratings");

           

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$lCbKDapgEqqJObRAbjjn5Oujzo1liP.TEj6WcMLxthi63eQbhAIoa");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Customers_CustomerId",
                table: "Ratings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
