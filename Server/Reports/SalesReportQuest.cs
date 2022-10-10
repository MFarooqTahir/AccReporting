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
            .Page(handler: page =>
            {
                page.Margin(value: 20);

                page.Size(pageSize: PageSizes.A4);
                page.Header().Element(handler: (container) =>
                {
                    var titleStyle = TextStyle.Default.FontSize(value: 20).FontFamily(value: "Calibri").SemiBold().Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                    container.Column(handler: column1 =>
                    {
                        column1.Item().Row(handler: row =>
                    {
                        row.ConstantItem(size: 150).Column(handler: column =>
                        {
                            column.Item().Text(text: $"Invoice #{ReportData?.InvNo}").Style(style: titleStyle);
                            column.Item().Text(text: $"Type: {ReportData?.Type}").FontFamily(value: "Calibri").SemiBold().Fallback(handler: x => x.FontFamily(value: "Fira Code"));

                            column.Item().Text(content: text =>
                            {
                                text.Span(text: "Issue date: ").FontFamily(value: "Calibri").SemiBold().Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                                if (ReportData?.Dated is not null)
                                {
                                    text.Span(text: (ReportData.Dated ?? DateTime.MinValue).ToString(format: "dd/MM/yyyy")).FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                                }
                                else
                                {
                                    text.Span(text: "-").FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                                }
                            });

                            column.Item().Text(content: text =>
                            {
                                text.Span(text: "Due date: ").FontFamily(value: "Calibri").SemiBold().Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                                if (ReportData?.DueDate is not null)
                                {
                                    text.Span(text: (ReportData.DueDate ?? DateTime.MinValue).ToString(format: "dd/MM/yyyy")).FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                                }
                                else
                                {
                                    text.Span(text: "-").FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                                }
                            });
                        });
                        row.RelativeItem().AlignCenter().Text(text: "Sales Report").FontFamily(value: "Calibri").ExtraBold().FontSize(value: 30).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        row.ConstantItem(size: 50);
                        row.ConstantItem(size: 100).Height(value: 50).Placeholder();
                    });
                        column1.Item().PaddingVertical(value: 5).LineHorizontal(size: 1).LineColor(value: Colors.Grey.Medium);
                    });
                });
                page.Content().Element(handler: ComposeContent);

                page.Footer().AlignCenter().Text(content: x =>
                {
                    x.CurrentPageNumber().FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                    x.Span(text: " / ").FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                    x.TotalPages().FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(value: 20).Column(handler: column =>
            {
                column.Spacing(value: 5);
                column.Item().Element(handler: x => NewHeadingRow(cont: x, val: ReportData?.CompanyName ?? ""));
                column.Item().Element(handler: x => NewSmallHeadingRow(cont: x, val: ReportData?.Address ?? ""));
                column.Item().Element(handler: x => NewSmallHeadingRow(cont: x, val: ReportData?.Cell ?? ""));
                column.Item().LineHorizontal(size: 1).LineColor(value: Colors.Grey.Medium);
                column.Item().Element(handler: x => NewDataRow(cont: x, title: "Ref. #: ", val: ReportData?.RefNumber ?? ""));
                column.Item().Element(handler: x => NewDataRow(cont: x, title: "Driver/Veh.: ", val: ReportData?.Driver ?? ""));
                column.Item().Element(handler: x => NewDataRow(cont: x, title: "Payment: ", val: ReportData?.Payment ?? ""));
                column.Item().PaddingVertical(value: 5).LineHorizontal(size: 1).LineColor(value: Colors.Grey.Medium);
                column.Item().Element(handler: ComposeTable);
                column.Item().AlignRight().Text(text: "Total Amount: " + (ReportData?.TotalBeforeDiscount ?? 0).ToString(format: "C2")).FontFamily(value: "Calibri").FontSize(value: 12).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                column.Item().AlignRight().Text(text: $"Invoice Discount: {(1 - ((ReportData?.TotalAfterDiscount ?? 0) / (ReportData?.TotalBeforeDiscount ?? 0))) * 100:F2} %").FontFamily(value: "Calibri").FontSize(value: 12).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                column.Item().AlignRight().Text(text: "Net Total: " + (ReportData?.TotalAfterDiscount ?? 0).ToString(format: "C2")).FontFamily(value: "Calibri").Bold().FontSize(value: 12).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
            });
        }

        private static void NewDataRow(IContainer cont, string title, string val)
        {
            cont.Row(handler: row =>
            {
                row.ConstantItem(size: 100).
                        Element(handler: x => x.Column(handler: col => col.Item().Text(text: title).FontFamily(value: "Calibri").Bold().Fallback(handler: x => x.FontFamily(value: "Fira Code"))));
                row.RelativeItem().
                Element(handler: x => x.Column(handler: col => col.Item().Text(text: val).FontFamily(value: "Calibri").Fallback(handler: x => x.FontFamily(value: "Fira Code"))));
            });
        }

        private static void NewHeadingRow(IContainer cont, string val)
        {
            cont.Row(handler: row =>
            {
                row.RelativeItem().
                Element(handler: x => x.AlignCenter().Column(handler: col => col.Item().Text(text: val).FontFamily(value: "Calibri").FontSize(value: 20).ExtraBold().Fallback(handler: x => x.FontFamily(value: "Fira Code"))));
            });
        }

        private static void NewSmallHeadingRow(IContainer cont, string val)
        {
            cont.Row(handler: row =>
            {
                row.RelativeItem().
                Element(handler: x => x.AlignCenter().Column(handler: col => col.Item().Text(text: val).FontFamily(value: "Calibri").FontSize(value: 12).Fallback(handler: x => x.FontFamily(value: "Fira Code"))));
            });
        }

        private void ComposeTable(IContainer container)
        {
            if (ReportData?.TableData is not null)
            {
                container.Table(handler: table =>
                {
                    table.ColumnsDefinition(handler: columns =>
                    {
                        columns.ConstantColumn(width: 25);
                        columns.RelativeColumn(width: 2);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(handler: header =>
                    {
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "#").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Description of Goods").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Brand").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Cartons/PCS").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Quantity").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Rate").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Amount").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Disc%").FontSize(value: 10);
                        header.Cell().Element(handler: CellStyle).AlignCenter().Text(text: "Net Amount").FontSize(value: 10);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .DefaultTextStyle(handler: x => x.FontFamily(value: "Calibri").SemiBold())
                                .PaddingVertical(value: 5).BorderBottom(value: 1).BorderColor(color: Colors.Black);
                        }
                    });
                    var count = 0;

                    foreach (var item in ReportData.TableData)
                    {
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: ++count).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Description).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Brand).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Pcs).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Quantity).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Rate).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Amount).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.Discount + " %").FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        table.Cell().Element(handler: CellStyle).AlignCenter().Text(text: item.NetAmount).FontFamily(value: "Calibri").FontSize(value: 10).Fallback(handler: x => x.FontFamily(value: "Fira Code"));
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Border(value: 1).BorderColor(color: Colors.Grey.Lighten2).PaddingVertical(value: 5);
                        }
                    }
                });
            }
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}