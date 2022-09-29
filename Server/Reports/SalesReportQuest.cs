using AccReporting.Shared.DTOs;

using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccReporting.Server.Reports
{
    public class SalesReportQuest : IDocument
    {
        public SalesReportDto? ReportData { get; set; }

        public SalesReportQuest(SalesReportDto reportData)
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
                    var titleStyle = TextStyle.Default.FontSize(20).FontFamily("Calibri").SemiBold().Fallback(x => x.FontFamily("Fira Code"));
                    container.Column(column1 =>
                    {
                        column1.Item().Row(row =>
                    {
                        row.ConstantItem(150).Column(column =>
                        {
                            column.Item().Text($"Invoice #{ReportData?.InvNo}").Style(titleStyle);
                            column.Item().Text($"Type: {ReportData?.Type}").FontFamily("Calibri").SemiBold().Fallback(x => x.FontFamily("Fira Code"));

                            column.Item().Text(text =>
                            {
                                text.Span("Issue date: ").FontFamily("Calibri").SemiBold().Fallback(x => x.FontFamily("Fira Code"));
                                if (ReportData?.Dated is not null)
                                {
                                    text.Span((ReportData.Dated ?? DateTime.MinValue).ToString("dd/MM/yyyy")).FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                                }
                                else
                                {
                                    text.Span("-").FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                                }
                            });

                            column.Item().Text(text =>
                            {
                                text.Span("Due date: ").FontFamily("Calibri").SemiBold().Fallback(x => x.FontFamily("Fira Code"));
                                if (ReportData?.DueDate is not null)
                                {
                                    text.Span((ReportData.DueDate ?? DateTime.MinValue).ToString("dd/MM/yyyy")).FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                                }
                                else
                                {
                                    text.Span("-").FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                                }
                            });
                        });
                        row.RelativeItem().AlignCenter().Text("Sales Report").FontFamily("Calibri").ExtraBold().FontSize(30).Fallback(x => x.FontFamily("Fira Code"));
                        row.ConstantItem(50);
                        row.ConstantItem(100).Height(50).Placeholder();
                    });
                        column1.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });
                });
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber().FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                    x.Span(" / ").FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                    x.TotalPages().FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"));
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(x => NewHeadingRow(x, ReportData?.CompanyName));
                column.Item().Element(x => NewSmallHeadingRow(x, ReportData?.Address));
                column.Item().Element(x => NewSmallHeadingRow(x, ReportData?.cell));
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                column.Item().Element(x => NewDataRow(x, "Ref. #: ", ReportData?.RefNumber));
                column.Item().Element(x => NewDataRow(x, "Driver/Veh.: ", ReportData?.Driver));
                column.Item().Element(x => NewDataRow(x, "Payment: ", ReportData?.Payment));
                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                column.Item().Element(ComposeTable);
                double netTotal = (ReportData?.tableData.Sum(x => x.NetAmount)) ?? 0;
                double Total = (ReportData?.tableData.Sum(x => x.Amount)) ?? 0;
                double dis = (1 - netTotal / Total) * 100;
                column.Item().AlignRight().Text($"Discount: {dis:F2} %").FontFamily("Calibri").FontSize(12).Fallback(x => x.FontFamily("Fira Code"));
                column.Item().AlignRight().Text("Total Amount: " + Total.ToString("C2")).FontFamily("Calibri").FontSize(12).Fallback(x => x.FontFamily("Fira Code"));
                column.Item().AlignRight().Text("Total After Discount: " + netTotal.ToString("C2")).FontFamily("Calibri").Bold().FontSize(12).Fallback(x => x.FontFamily("Fira Code"));
            });
        }

        private static void NewDataRow(IContainer cont, string title, string val)
        {
            cont.Row(row =>
            {
                row.ConstantItem(100).
                        Element(x => x.Column(col => col.Item().Text(title).FontFamily("Calibri").Bold().Fallback(x => x.FontFamily("Fira Code"))));
                row.RelativeItem().
                Element(x => x.Column(col => col.Item().Text(val).FontFamily("Calibri").Fallback(x => x.FontFamily("Fira Code"))));
            });
        }

        private static void NewHeadingRow(IContainer cont, string val)
        {
            cont.Row(row =>
            {
                row.RelativeItem().
                Element(x => x.AlignCenter().Column(col => col.Item().Text(val).FontFamily("Calibri").FontSize(20).ExtraBold().Fallback(x => x.FontFamily("Fira Code"))));
            });
        }

        private static void NewSmallHeadingRow(IContainer cont, string val)
        {
            cont.Row(row =>
            {
                row.RelativeItem().
                Element(x => x.AlignCenter().Column(col => col.Item().Text(val).FontFamily("Calibri").FontSize(12).Fallback(x => x.FontFamily("Fira Code"))));
            });
        }

        private void ComposeTable(IContainer container)
        {
            if (ReportData.tableData is not null)
            {
                container.Table(table =>
                {
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
                                .DefaultTextStyle(x => x.FontFamily("Calibri").SemiBold())
                                .PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });
                    int count = 0;

                    foreach (var item in ReportData.tableData)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text(++count).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Description).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Brand).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Pcs).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Rate).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Amount).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Discount + " %").FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.NetAmount).FontFamily("Calibri").FontSize(10).Fallback(x => x.FontFamily("Fira Code"));
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Border(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });
            }
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}