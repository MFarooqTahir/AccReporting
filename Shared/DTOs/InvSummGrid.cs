using AccReporting.Shared.ContextModels;

namespace AccReporting.Shared.DTOs
{
    public class InvSummGridModel
    {
        public InvSummGridModel(InvSumm inv, double? amount)
        {
            InvNo = inv.InvNo;
            Type = inv.Type;
            Amount = amount ?? 0;
        }

        public int? InvNo { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
    }
}