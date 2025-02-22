using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNSHOP.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsVerified_To_Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "CustomerNotifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsVerified", "Password" },
                values: new object[] { false, "$2a$11$KtqLMFb.sMl1YK/dM1bKDOD3s3ih4I8irHqkkB6bPQVW0.9S090dG" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsVerified", "Password" },
                values: new object[] { false, "$2a$11$F9fDbvpV3UUnHxXUllLp7uZpdVClZQiP1Ke7H9b/a8ML2nRgrecq6" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsVerified", "Password" },
                values: new object[] { false, "$2a$11$5GqnosrWGf4NgpXdVsgl7usWkDNmxawnueq/heqs5.Fq0.nOUoS0m" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsVerified", "Password" },
                values: new object[] { false, "$2a$11$e.NzQLJCtqm40pgZVQJR8OsREFpRqvCHCfUlvgTSzePYA5B3VePwm" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsVerified", "Password" },
                values: new object[] { false, "$2a$11$PgCCN3J2dsV6zgwBOAlsFetyV0iHLPNlHY3wwfjViH0uR/o3dwvPO" });

            migrationBuilder.UpdateData(
                table: "CustomerNotifications",
                keyColumns: new[] { "CustomerId", "NotificationId" },
                keyValues: new object[] { 1, 1 },
                column: "IsRead",
                value: false);

            migrationBuilder.UpdateData(
                table: "CustomerNotifications",
                keyColumns: new[] { "CustomerId", "NotificationId" },
                keyValues: new object[] { 1, 2 },
                column: "IsRead",
                value: false);

            migrationBuilder.UpdateData(
                table: "CustomerNotifications",
                keyColumns: new[] { "CustomerId", "NotificationId" },
                keyValues: new object[] { 2, 3 },
                column: "IsRead",
                value: false);

            migrationBuilder.UpdateData(
                table: "CustomerNotifications",
                keyColumns: new[] { "CustomerId", "NotificationId" },
                keyValues: new object[] { 2, 4 },
                column: "IsRead",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "CustomerNotifications");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$5gFLfj.bXdEK2KSIhOwh8u2bammAdkjjhc9c3TaQsPQ1qoWiJqQ5W");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$/vw9fH8V3R3jGvna/0qd7ej4VMeXPLSy1ZNsLeyi3cdNnwgZXResm");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$i5QF0H60zDZ0fyXBKz4Kieo3lALYq0MAc0gg2pMSMdRoUmqDoQ8x2");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$ZOwm6XhayPFsewbgj9AwrOTXvaO6gdqxXLRQbfbeXTIrn3xGt5z/y");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$bPKmpaW4vMEeGpNOvWqBp.bb5E7/lAhc.ACpFTeaA8JjS3mUYmSs6");
        }
    }
}
