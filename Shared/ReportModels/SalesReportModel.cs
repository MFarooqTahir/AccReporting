namespace AccountsReportsWASM.Shared.ReportModels
{
    public class SalesReportModel
    {
        public SalesReportModel()
        {

        }
        public SalesReportModel(int sno, string description, string brand, int pcs, int quantity, float rate, float amount, float discount, float netAmount)
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
        }

        public int Sno { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public int Pcs { get; set; }
        public int Quantity { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        public float Discount { get; set; }
        public float NetAmount { get; set; }

    }
}
