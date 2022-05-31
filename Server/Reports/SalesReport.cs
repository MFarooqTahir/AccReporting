using AccReporting.Shared.DTOs;

using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccReporting.Server.Reports
{
    public class SalesReport : IDocument
    {
        public SalesReportDto ReportData { get; set; }

        public SalesReport()
        {
        }

        public SalesReport(SalesReportDto reportData)
        {
            ReportData = reportData;
        }

        public void Compose(IDocumentContainer container)
        {
            container
            .Page(page =>
            {
                page.Margin(20);

                page.Size(PageSizes.A4);
                page.Header().Element((container) =>
                {
                    var titleStyle = TextStyle.Default.FontSize(20).SemiBold();
                    container.Column(column1 =>
                    {


                        column1.Item().Row(row =>
                    {
                        row.ConstantItem(150).Column(column =>
                        {
                            column.Item().Text($"Invoice #{ReportData.InvNo}").Style(titleStyle);

                            column.Item().Text(text =>
                            {
                                text.Span("Issue date: ").SemiBold();
                                if (ReportData.Dated is not null)
                                {
                                    text.Span((ReportData.Dated ?? DateTime.MinValue).ToString("dd/MM/yyyy"));
                                }
                                else
                                {
                                    text.Span("-");
                                }
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("Due date: ").SemiBold();
                                if (ReportData.DueDate is not null)
                                {
                                    text.Span((ReportData.DueDate ?? DateTime.MinValue).ToString("dd/MM/yyyy"));
                                }
                                else
                                {
                                    text.Span("-");
                                }
                            });
                        });
                        row.RelativeItem().AlignCenter().Text("Sales Report").ExtraBold().FontSize(30);
                        row.ConstantItem(50);
                        row.ConstantItem(100).Height(50).Placeholder();

                    });
                        column1.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });
                });
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(x => NewHeadingRow(x, ReportData.CompanyName));
                column.Item().Element(x => NewHeadingRow(x, ReportData.Address));
                column.Item().Element(x => NewHeadingRow(x, ReportData.cell));

                column.Item().Element(x => newDataRow(x, "Ref. #: ", ReportData.RefNumber));
                column.Item().Element(x => newDataRow(x, "Driver/Veh.: ", ReportData.Driver));
                column.Item().Element(x => newDataRow(x, "Buyer: ", ReportData.NameAndAddress));
                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                column.Item().Element(ComposeTable);
                column.Item().AlignRight().Text("Grand Total: " + ReportData.Total).Bold().FontSize(15);
            });
        }

        private void newDataRow(IContainer cont, string title, string val)
        {
            cont.Row(row =>
            {
                row.ConstantItem(100).
                        Element(x =>
                        {
                            x.Column(col =>
                            {
                                col.Item().Text(title).Bold();
                            });
                        });
                row.RelativeItem().
                Element(x =>
                {
                    x.Column(col =>
                    {
                        col.Item().Text(val);
                    });
                });
            });
        }
        private void NewHeadingRow(IContainer cont, string val)
        {
            cont.Row(row =>
            {
                row.RelativeItem().
                Element(x =>
                {
                    x.AlignCenter().Column(col =>
                    {
                        col.Item().Text(val).FontSize(20).ExtraBold();
                    });
                });
            });
        }
        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignCenter().Text("#").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Description of Goods").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Brand").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Cartons/PCS").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Quantity").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Rate").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Amount").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Disc%").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Net Amount").FontSize(10);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .DefaultTextStyle(x => x.SemiBold())
                            .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });
                int count = 0;

                if (ReportData.tableData is not null)
                {
                    foreach (var item in ReportData.tableData)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text(++count).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Description).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Brand).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Pcs).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Rate).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Amount).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Discount).FontSize(10);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.NetAmount).FontSize(10);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Border(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                }
            });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}