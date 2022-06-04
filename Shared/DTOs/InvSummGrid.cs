using AccReporting.Shared.ContextModels;

namespace AccReporting.Shared.DTOs
{
    public class InvSummGridModel
    {
        public InvSummGridModel(InvSumm inv, IDictionary<int, double?> amount)
        {
            invNo = inv.InvNo;
            Type = inv.Type;
            amount.TryGetValue(inv.InvNo ?? 0, out var amt);
            Amount = amt ?? 0;
        }

        public int? invNo { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
    }
}