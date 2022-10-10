using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccReporting.Server.Migrations.AccountInfoDb
{
    public partial class AddedIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GST",
                table: nameof(Acfile),
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true)
                .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                .OldAnnotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ActCodeIndex",
                table: nameof(Trans),
                column: "ActCode");

            migrationBuilder.CreateIndex(
                name: "InvNoIndex",
                table: nameof(InvSumm),
                column: "InvNo");

            migrationBuilder.CreateIndex(
                name: "ItemCodeIndex",
                table: nameof(Inventory),
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "InvNoIndex",
                table: nameof(InvDet),
                column: "InvNo");

            migrationBuilder.CreateIndex(
                name: "SpIndex",
                table: nameof(InvDet),
                column: "SP");

            migrationBuilder.CreateIndex(
                name: "CodeIndex",
                table: nameof(Category),
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "ActCodeIndex",
                table: nameof(Basic),
                column: "ActCode");

            migrationBuilder.CreateIndex(
                name: "ActCodeIndex",
                table: nameof(Acfile),
                column: "ActCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ActCodeIndex",
                table: nameof(Trans));

            migrationBuilder.DropIndex(
                name: "InvNoIndex",
                table: nameof(InvSumm));

            migrationBuilder.DropIndex(
                name: "ItemCodeIndex",
                table: nameof(Inventory));

            migrationBuilder.DropIndex(
                name: "InvNoIndex",
                table: nameof(InvDet));

            migrationBuilder.DropIndex(
                name: "SpIndex",
                table: nameof(InvDet));

            migrationBuilder.DropIndex(
                name: "CodeIndex",
                table: nameof(Category));

            migrationBuilder.DropIndex(
                name: "ActCodeIndex",
                table: nameof(Basic));

            migrationBuilder.DropIndex(
                name: "ActCodeIndex",
                table: nameof(Acfile));

            migrationBuilder.AlterColumn<string>(
                name: "GST",
                table: nameof(Acfile),
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                .OldAnnotation(name: "MySql:CharSet", value: "utf8mb4");
        }
    }
}
