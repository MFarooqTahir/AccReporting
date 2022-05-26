using AccReporting.Shared.DTOs;

using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccReporting.Client.Reports
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
                page.Margin(25);

                page.Size(PageSizes.A4);
                page.Header().Element((x) =>
                {
                    x.AlignCenter().Text("Sales Report").FontSize(30).ExtraBold();
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
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(x =>
                {
                    x.Row(row =>
                    {
                        row.ConstantItem(150).
                        Element(x =>
                        {
                            x.Column(col =>
                            {
                                col.Item().Text("Name and Address: ").Bold();
                            });
                        });
                        row.RelativeItem().
                        Element(x =>
                        {
                            x.Column(col =>
                            {
                                col.Item().Text(ReportData.NameAndAddress);
                            });
                        });
                    });
                });
                column.Item().Element(x => newDataRow(x, "Company Name: ", ReportData.CompanyName));
                column.Item().Element(ComposeTable);
            });
        }

        private void newDataRow(IContainer cont, string title, string val)
        {
            cont.Row(row =>
            {
                row.ConstantItem(150).
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

        private void ComposeTable(IContainer container)
        {
            container
            .Height(250)
            .Background(Colors.Grey.Lighten3)
            .AlignCenter()
            .AlignMiddle()
            .Text("Table").FontSize(16);
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}