namespace AccReporting.Shared.DTOs
{
    public class InvSummGridModel
    {


        public InvSummGridModel()
        {
        }

        public InvSummGridModel(int? invNo, string type, double? amount)
        {
            InvNo = invNo;
            Type = type;
            Amount = amount;
        }

        public int? InvNo { get; set; }

        public string Type { get; set; }

        public double? Amount { get; set; }
    }
}