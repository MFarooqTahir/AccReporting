using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccReporting.Server.Migrations.ApplicationDb
{
    public partial class Change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "774b00dd-3705-45b6-aaa9-7e96975ba946");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94acdeed-10a6-45ec-9454-a215540f44ab");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9d7af01c-0293-46c4-8b01-3f76f7a16863");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelected",
                table: "CompanyAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "351d79bc-c0b7-4a0e-b131-019b9878ecd0", "a664caa2-3794-4bb9-b671-959f43d35521", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e5e8863c-eafe-49c5-8465-24dc6e604656", "e3a04c2e-1497-4097-b02f-47e43d624e7e", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "351d79bc-c0b7-4a0e-b131-019b9878ecd0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5e8863c-eafe-49c5-8465-24dc6e604656");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelected",
                table: "CompanyAccounts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "774b00dd-3705-45b6-aaa9-7e96975ba946", "7b06b426-7a4f-4aac-8237-9db1d8f2f882", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "94acdeed-10a6-45ec-9454-a215540f44ab", "067bfaaf-b1dd-4377-841e-f59b8050b500", "CompanyAdmin", "COMPANYADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9d7af01c-0293-46c4-8b01-3f76f7a16863", "1f3f9e4e-590e-4692-baa8-43e98b42b15b", "User", "USER" });
        }
    }
}
