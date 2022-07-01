using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccReporting.Server.Migrations
{
    public partial class Approval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34d8c73a-cf67-4739-9890-ef4b9375e00f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a122f09a-2b50-4609-a5d4-89ea9430db86");

            migrationBuilder.AlterColumn<string>(
                name: "DbName",
                table: "Companies",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Companies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "13643671-6dac-4ae5-ad4a-4b8b3cf49a66", "e7e6bb6e-4b01-4404-a109-b3b33289fcd2", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a5862cff-2526-44bb-bdd4-53385912d57a", "1f95adad-1edd-474a-93f7-5b2de0fdb85c", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13643671-6dac-4ae5-ad4a-4b8b3cf49a66");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5862cff-2526-44bb-bdd4-53385912d57a");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "DbName",
                table: "Companies",
                type: "varchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "34d8c73a-cf67-4739-9890-ef4b9375e00f", "fbab6438-e19c-41f3-916d-78549a5df59b", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a122f09a-2b50-4609-a5d4-89ea9430db86", "80ec5576-4ae0-4ed5-ae78-698767f5929c", "User", "USER" });
        }
    }
}
