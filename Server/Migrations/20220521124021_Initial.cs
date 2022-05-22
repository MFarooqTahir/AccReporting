using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccReporting.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Acfile",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ActName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    fax = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    OpBal = table.Column<double>(type: "float", nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GST = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CrDays = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Acfile__B87C5B5F3831C82B", x => x.IDPr);
                });

            migrationBuilder.CreateTable(
                name: "Basic",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ActName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sno = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Basic__B87C5B5FF0F4AE13", x => x.IDPr);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Item = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Standard = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    JoiningType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Temp1 = table.Column<int>(type: "int", nullable: true),
                    Temp2 = table.Column<int>(type: "int", nullable: true),
                    Temp3 = table.Column<int>(type: "int", nullable: true),
                    Date1 = table.Column<DateTime>(type: "datetime", nullable: true),
                    Temp4 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Temp5 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Temp6 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Temp7 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__B87C5B5F9899F9B0", x => x.IDPr);
                });

            migrationBuilder.CreateTable(
                name: "InvDet",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvNo = table.Column<int>(type: "int", nullable: true),
                    InvDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ICode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Qty = table.Column<double>(type: "float", nullable: true),
                    Qty2 = table.Column<double>(type: "float", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Packing = table.Column<double>(type: "float", nullable: true),
                    Rate = table.Column<double>(type: "float", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    NetAmount = table.Column<double>(type: "float", nullable: true),
                    SP = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    pName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Pressure = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CateCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Dper = table.Column<double>(type: "float", nullable: true),
                    RegionCode = table.Column<int>(type: "int", nullable: true),
                    RegionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FILE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InvDet__B87C5B5F67F5802F", x => x.IDPr);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ItemDescrip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MfcCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ManuName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Pressure = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Length = table.Column<double>(type: "float", nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    RetPrice = table.Column<decimal>(type: "money", nullable: true),
                    RetPrice2 = table.Column<decimal>(type: "money", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    OpBal = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventor__B87C5B5F00A47D01", x => x.IDPr);
                });

            migrationBuilder.CreateTable(
                name: "InvSumm",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InvNo = table.Column<int>(type: "int", nullable: true),
                    InvDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TotBill = table.Column<double>(type: "float", nullable: true),
                    Built = table.Column<double>(type: "float", nullable: true),
                    DisPer = table.Column<double>(type: "float", nullable: true),
                    Dis = table.Column<double>(type: "float", nullable: true),
                    Ser = table.Column<double>(type: "float", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    cartage = table.Column<double>(type: "float", nullable: true),
                    AddLess = table.Column<double>(type: "float", nullable: true),
                    CrDays = table.Column<int>(type: "int", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RefNo = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Payment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Delivery = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InvSumm__B87C5B5F9C9191C6", x => x.IDPr);
                });

            migrationBuilder.CreateTable(
                name: "Trans",
                columns: table => new
                {
                    IDPr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ActName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    TransAmt = table.Column<double>(type: "float", nullable: true),
                    Vnoc = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Vnon = table.Column<int>(type: "int", nullable: true),
                    Des = table.Column<string>(type: "nvarchar(205)", maxLength: 205, nullable: true),
                    ChqNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ChqDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Trans__B87C5B5F2A4B3E66", x => x.IDPr);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acfile");

            migrationBuilder.DropTable(
                name: "Basic");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "InvDet");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "InvSumm");

            migrationBuilder.DropTable(
                name: "Trans");
        }
    }
}
