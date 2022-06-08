namespace AccReporting.Shared.DTOs
{
    public class InvSummGridModel
    {
        public InvSummGridModel()
        {
        }

        public InvSummGridModel(int? invNo, string type, double? amount, double? netAmount)
        {
            InvNo = invNo;
            Type = type;
            Amount = amount;
            NetAmount = netAmount;
        }

        public int? InvNo { get; set; }

        public string Type { get; set; }

        public double? Amount { get; set; }
        public double? NetAmount { get; set; }
    }
}