using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNSHOP.Migrations
{
    /// <inheritdoc />
    public partial class AddResetTokenToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "ResetToken", "ResetTokenExpiry" },
                values: new object[] { "$2a$11$n7AkzN/bBRqYCFjOyMpI3ONUvUUnIbn3KsjIa8GW14HcYHmNJMe7y", null, null });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "ResetToken", "ResetTokenExpiry" },
                values: new object[] { "$2a$11$cfE22zZ96Rxa6a.jQCptLe3Y7XI8SIrdFSsEi1mL9wO.CxbjAaBQ.", null, null });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Password", "ResetToken", "ResetTokenExpiry" },
                values: new object[] { "$2a$11$jtxrAx2tNiTdLvW4DqPTJ.I3qhjlwvxRjh3B4a7Og1lyajl3ko9Bi", null, null });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Password", "ResetToken", "ResetTokenExpiry" },
                values: new object[] { "$2a$11$QgWTIxrMm63wPKFZOApjnueSJx/rTK82y4k.CaRFmL1aEvbFq8q3i", null, null });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Password", "ResetToken", "ResetTokenExpiry" },
                values: new object[] { "$2a$11$wcwBbXaMztM1x2iFnmDxNuG5FjluyhVnLimVh0PXVqj7.CT7sUq2y", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$fSehxbm6vSGy43YQWOsayu9/qeBO3KXm6w4kmUbj1Uo1GvFjN9eDe");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$5GGmn4TM3TkgeIz/X4zaVuLnYLTPK4aUB/YuCmdqcyez68F8bja4m");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$i2r7hTm5aELzNbiXUqQ.OebPJJwTqezeb/3VGmPI4Mf806XDI.tdi");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$lMj674LMS3b/wHL32v/Z5.45DSbru2MjqlzsIYDCqxnE127w2XFT.");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$y1xpJA7qQsCZJRuxGBTj.eT5dwwCg.r7XWWfrWL/FgQPOTQjZT61e");
        }
    }
}
