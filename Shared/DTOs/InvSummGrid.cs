using AccReporting.Shared.ContextModels;

namespace AccReporting.Shared.DTOs
{
    public class InvSummGridModel
    {
        public class dba
        {
            public dba()
            {
            }

            public dba(string type, double? amount)
            {
                this.type = type;
                this.amount = amount;
            }

            public string type { get; set; }
            public double? amount { get; set; }
        }

        public InvSummGridModel(InvSumm inv, IDictionary<int, dba> amtt)
        {
            InvNo = inv.InvNo;
            amtt.TryGetValue(InvNo ?? 0, out dba amt);
            Amount = amt.amount ?? 0;
            Type = amt.type;
        }

        public InvSummGridModel()
        {
        }

        public int? InvNo { get; set; }

        public string Type { get; set; }

        public double Amount { get; set; }
    }
}