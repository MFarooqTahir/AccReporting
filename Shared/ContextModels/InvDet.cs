﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using AccReporting.Shared.Helpers;

using System;
using System.Collections.Generic;

namespace AccReporting.Shared.ContextModels
{
    public partial class InvDet
    {
        public InvDet(string[] x)
        {

            InvNo = ValOrDefault.ToInt(x[0]);
            InvDate = ValOrDefault.ToDateTime(x[1]);
            Pcode = x[2];
            Icode = x[3];
            Iname = x[4];
            Qty = ValOrDefault.ToDouble(x[5]);
            Qty2 = ValOrDefault.ToDouble(x[6]);
            Unit = x[7];
            Packing = ValOrDefault.ToDouble(x[8]);
            Rate = ValOrDefault.ToDouble(x[9]);
            Amount = ValOrDefault.ToDouble(x[10]);
            NetAmount = ValOrDefault.ToDouble(x[11]);
            Sp = x[12];
            Type = x[13];
            PName = x[14];
            Size = x[15];
            Pressure = x[16];
            CateCode = x[17];
            Dper = ValOrDefault.ToDouble(x[18]);
            RegionCode = ValOrDefault.ToInt(x[19]);
            RegionName = x[20];
            File = x[21];
        }
        public InvDet()
        {
            
        }
        public int Idpr { get; set; }
        public int? InvNo { get; set; }
        public DateTime? InvDate { get; set; }
        public string Pcode { get; set; }
        public string Icode { get; set; }
        public string Iname { get; set; }
        public double? Qty { get; set; }
        public double? Qty2 { get; set; }
        public string Unit { get; set; }
        public double? Packing { get; set; }
        public double? Rate { get; set; }
        public double? Amount { get; set; }
        public double? NetAmount { get; set; }
        public string Sp { get; set; }
        public string Type { get; set; }
        public string PName { get; set; }
        public string Size { get; set; }
        public string Pressure { get; set; }
        public string CateCode { get; set; }
        public double? Dper { get; set; }
        public int? RegionCode { get; set; }
        public string RegionName { get; set; }
        public string File { get; set; }
    }
}