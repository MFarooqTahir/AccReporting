﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using AccReporting.Shared.Helpers;

using MySqlConnector;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccReporting.Shared.ContextModels
{
    public partial class Inventory
    {
        public Inventory(string[] x)
        {
            ItemCode = MySqlHelper.EscapeString(x[0]);
            ItemDescrip = MySqlHelper.EscapeString(x[1]);
            MfcCode = MySqlHelper.EscapeString(x[2]);
            ManuName = MySqlHelper.EscapeString(x[3]);
            Size = MySqlHelper.EscapeString(x[4]);
            Pressure = MySqlHelper.EscapeString(x[5]);
            Length = ValOrDefault.ToDouble(MySqlHelper.EscapeString(x[6]));
            Price = ValOrDefault.ToDecimal(MySqlHelper.EscapeString(x[7]));
            RetPrice = ValOrDefault.ToDecimal(MySqlHelper.EscapeString(x[8]));
            RetPrice2 = ValOrDefault.ToDecimal(MySqlHelper.EscapeString(x[9]));
            Unit = MySqlHelper.EscapeString(x[10]);
            OpBal = ValOrDefault.ToDouble(MySqlHelper.EscapeString(x[11]));
        }
        public Inventory()
        {
            
        }
        public string ToInsert()
        {
            return $"('{ItemCode}','{ItemDescrip}','{MfcCode}','{ManuName}','{Size}','{Pressure}',{Length ?? 0},{Price ?? 0},{RetPrice ?? 0},{RetPrice2 ?? 0},'{Unit}',{OpBal ?? 0})";
        }
        public int Idpr { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescrip { get; set; }
        public string MfcCode { get; set; }
        public string ManuName { get; set; }
        public string Size { get; set; }
        public string Pressure { get; set; }
        public double? Length { get; set; }
        [Column(TypeName="decimal(10,2)")]
        public decimal? Price { get; set; }
        [Column(TypeName="decimal(10,2)")]
        public decimal? RetPrice { get; set; }
        [Column(TypeName="decimal(10,2)")]
        public decimal? RetPrice2 { get; set; }
        public string Unit { get; set; }
        public double? OpBal { get; set; }
    }
}