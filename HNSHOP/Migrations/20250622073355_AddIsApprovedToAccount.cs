using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNSHOP.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VerifyToken",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsApproved", "Password" },
                values: new object[] { false, "$2a$11$fSehxbm6vSGy43YQWOsayu9/qeBO3KXm6w4kmUbj1Uo1GvFjN9eDe" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsApproved", "Password" },
                values: new object[] { false, "$2a$11$5GGmn4TM3TkgeIz/X4zaVuLnYLTPK4aUB/YuCmdqcyez68F8bja4m" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsApproved", "Password" },
                values: new object[] { false, "$2a$11$i2r7hTm5aELzNbiXUqQ.OebPJJwTqezeb/3VGmPI4Mf806XDI.tdi" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsApproved", "Password" },
                values: new object[] { false, "$2a$11$lMj674LMS3b/wHL32v/Z5.45DSbru2MjqlzsIYDCqxnE127w2XFT." });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsApproved", "Password" },
                values: new object[] { false, "$2a$11$y1xpJA7qQsCZJRuxGBTj.eT5dwwCg.r7XWWfrWL/FgQPOTQjZT61e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "VerifyToken",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$KtqLMFb.sMl1YK/dM1bKDOD3s3ih4I8irHqkkB6bPQVW0.9S090dG");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$F9fDbvpV3UUnHxXUllLp7uZpdVClZQiP1Ke7H9b/a8ML2nRgrecq6");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$5GqnosrWGf4NgpXdVsgl7usWkDNmxawnueq/heqs5.Fq0.nOUoS0m");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$e.NzQLJCtqm40pgZVQJR8OsREFpRqvCHCfUlvgTSzePYA5B3VePwm");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$PgCCN3J2dsV6zgwBOAlsFetyV0iHLPNlHY3wwfjViH0uR/o3dwvPO");
        }
    }
}
