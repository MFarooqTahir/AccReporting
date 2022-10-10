using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccReporting.Server.Migrations.AccountInfoDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(Acfile),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    ActCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    ActName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Address1 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Address2 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Address3 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    CrDays = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    fax = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    GST = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    OpBal = table.Column<double>(type: "double", nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__Acfile__B87C5B5F3831C82B", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(Basic),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    ActCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    ActName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Sno = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__Basic__B87C5B5FF0F4AE13", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(Category),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Color = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Date1 = table.Column<DateTime>(type: "datetime", nullable: true),
                    Item = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    JoiningType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Origin = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Standard = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Temp1 = table.Column<int>(type: "int", nullable: true),
                    Temp2 = table.Column<int>(type: "int", nullable: true),
                    Temp3 = table.Column<int>(type: "int", nullable: true),
                    Temp4 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Temp5 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Temp6 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Temp7 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__Category__B87C5B5F9899F9B0", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(InvDet),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(type: "double", nullable: true),
                    CateCode = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Dper = table.Column<double>(type: "double", nullable: true),
                    FILE = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    ICode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    IName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    InvDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvNo = table.Column<int>(type: "int", nullable: true),
                    NetAmount = table.Column<double>(type: "double", nullable: true),
                    pName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Packing = table.Column<double>(type: "double", nullable: true),
                    PCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Pressure = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Qty = table.Column<double>(type: "double", nullable: true),
                    Qty2 = table.Column<double>(type: "double", nullable: true),
                    Rate = table.Column<double>(type: "double", nullable: true),
                    RegionCode = table.Column<int>(type: "int", nullable: true),
                    RegionName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Size = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    SP = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Unit = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__InvDet__B87C5B5F67F5802F", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(Inventory),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    ItemCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    ItemDescrip = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Length = table.Column<double>(type: "double", nullable: true),
                    ManuName = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    MfcCode = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    OpBal = table.Column<double>(type: "double", nullable: true),
                    Pressure = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RetPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RetPrice2 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Size = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Unit = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__Inventor__B87C5B5F00A47D01", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(InvSumm),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    AddLess = table.Column<double>(type: "double", nullable: true),
                    Built = table.Column<double>(type: "double", nullable: true),
                    cartage = table.Column<double>(type: "double", nullable: true),
                    CrDays = table.Column<int>(type: "int", nullable: true),
                    Delivery = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Dis = table.Column<double>(type: "double", nullable: true),
                    DisPer = table.Column<double>(type: "double", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    HCode = table.Column<int>(type: "int", nullable: true),
                    InvDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvNo = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    OrderNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Payment = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    PCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    PName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    RefNo = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Remarks = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Ser = table.Column<double>(type: "double", nullable: true),
                    TotBill = table.Column<double>(type: "double", nullable: true),
                    Type = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__InvSumm__B87C5B5F9C9191C6", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");

            migrationBuilder.CreateTable(
                name: nameof(Trans),
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation(name: "MySql:ValueGenerationStrategy", value: MySqlValueGenerationStrategy.IdentityColumn),
                    ActCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    ActName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    ChqDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ChqNo = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Des = table.Column<string>(type: "varchar(205)", maxLength: 205, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    TransAmt = table.Column<double>(type: "double", nullable: true),
                    Vnoc = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true)
                        .Annotation(name: "MySql:CharSet", value: "utf8mb4"),
                    Vnon = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK__Trans__B87C5B5F2A4B3E66", columns: x => x.IDPr);
                })
                .Annotation(name: "MySql:CharSet", value: "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: nameof(Acfile));

            migrationBuilder.DropTable(
                name: nameof(Basic));

            migrationBuilder.DropTable(
                name: nameof(Category));

            migrationBuilder.DropTable(
                name: nameof(InvDet));

            migrationBuilder.DropTable(
                name: nameof(Inventory));

            migrationBuilder.DropTable(
                name: nameof(InvSumm));

            migrationBuilder.DropTable(
                name: nameof(Trans));
        }
    }
}
