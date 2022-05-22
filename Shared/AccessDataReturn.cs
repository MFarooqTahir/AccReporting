using AccReporting.Shared.ContextModels;

namespace AccReporting.Shared
{
    public class AccessDataReturn
    {
        public string Dbname { get; set; }
        public List<Acfile> AcfileList { get; set; } = new();
        public List<Trans> TransList { get; set; } = new();
        public List<Basic> BasicList { get; set; } = new();
        public List<Category> CategoryList { get; set; } = new();
        public List<InvDet> InvDetList { get; set; } = new();
        public List<Inventory> InventoryList { get; set; } = new();
        public List<InvSumm> InvSummList { get; set; } = new();

        public AccessDataReturn()
        {
        }

        public AccessDataReturn(string dbname, List<Acfile> acfileList, List<Trans> transList, List<Basic> basicList, List<Category> categoryList, List<InvDet> invDetList, List<Inventory> inventoryList, List<InvSumm> invSummList)
        {
            Dbname = dbname;
            AcfileList = acfileList;
            TransList = transList;
            BasicList = basicList;
            CategoryList = categoryList;
            InvDetList = invDetList;
            InventoryList = inventoryList;
            InvSummList = invSummList;
        }
    }
}