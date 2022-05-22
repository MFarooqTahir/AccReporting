using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccReporting.Server.Migrations.ApplicationDb
{
    public partial class InitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
