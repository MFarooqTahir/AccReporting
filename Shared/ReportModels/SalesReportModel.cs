namespace AccountsReportsWASM.Shared.ReportModels
{
    public class SalesReportModel
    {
        public SalesReportModel()
        {

        }
        public SalesReportModel(int sno, string description, string brand, int pcs, int quantity, float rate, float amount, float discount, float netAmount, string unit)
        {
            Sno = sno;
            Description = description;
            Brand = brand;
            Pcs = pcs;
            Quantity = quantity;
            Rate = rate;
            Amount = amount;
            Discount = discount;
            NetAmount = netAmount;
            this.Unit = unit;
        }

        public int? Sno { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public int Pcs { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public double? Rate { get; set; }
        public double? Amount { get; set; }
        public double? Discount { get; set; }
        public double? NetAmount { get; set; }

    }
}
