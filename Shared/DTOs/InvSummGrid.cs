namespace AccReporting.Shared.DTOs
{
    public class InvSummGridModel
    {
        public InvSummGridModel()
        {
        }
        public InvSummGridModel(int? invNo, double? amount, double? netAmount, string name, string pcode)
        {
            InvNo = invNo;
            Amount = amount;
            NetAmount = netAmount;
            Name = name;
            PCode = pcode;
        }

        public int? InvNo { get; set; }

        public string Name { get; set; }
        public string PCode { get; set; }

        public double? Amount { get; set; }
        public double? NetAmount { get; set; }
    }
}