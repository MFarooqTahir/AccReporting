﻿// <auto-generated />
using System;
using System.Reflection;
using AccReporting.Shared.ContextModels;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace AccReporting.Server.OptimizedModels
{
    internal partial class InventoryEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "AccReporting.Shared.ContextModels.Inventory",
                typeof(Inventory),
                baseEntityType);

            var idpr = runtimeEntityType.AddProperty(
                "Idpr",
                typeof(int),
                propertyInfo: typeof(Inventory).GetProperty("Idpr", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<Idpr>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);
            idpr.AddAnnotation("Relational:ColumnName", "IDPr");

            var itemCode = runtimeEntityType.AddProperty(
                "ItemCode",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("ItemCode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<ItemCode>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 30);

            var itemDescrip = runtimeEntityType.AddProperty(
                "ItemDescrip",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("ItemDescrip", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<ItemDescrip>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var length = runtimeEntityType.AddProperty(
                "Length",
                typeof(double?),
                propertyInfo: typeof(Inventory).GetProperty("Length", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<Length>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var manuName = runtimeEntityType.AddProperty(
                "ManuName",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("ManuName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<ManuName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 35);

            var mfcCode = runtimeEntityType.AddProperty(
                "MfcCode",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("MfcCode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<MfcCode>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 4);

            var opBal = runtimeEntityType.AddProperty(
                "OpBal",
                typeof(double?),
                propertyInfo: typeof(Inventory).GetProperty("OpBal", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<OpBal>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var pressure = runtimeEntityType.AddProperty(
                "Pressure",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("Pressure", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<Pressure>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 10);

            var price = runtimeEntityType.AddProperty(
                "Price",
                typeof(decimal?),
                propertyInfo: typeof(Inventory).GetProperty("Price", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<Price>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            price.AddAnnotation("Relational:ColumnType", "decimal(10,2)");

            var retPrice = runtimeEntityType.AddProperty(
                "RetPrice",
                typeof(decimal?),
                propertyInfo: typeof(Inventory).GetProperty("RetPrice", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<RetPrice>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            retPrice.AddAnnotation("Relational:ColumnType", "decimal(10,2)");

            var retPrice2 = runtimeEntityType.AddProperty(
                "RetPrice2",
                typeof(decimal?),
                propertyInfo: typeof(Inventory).GetProperty("RetPrice2", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<RetPrice2>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            retPrice2.AddAnnotation("Relational:ColumnType", "decimal(10,2)");

            var size = runtimeEntityType.AddProperty(
                "Size",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("Size", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<Size>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 15);

            var unit = runtimeEntityType.AddProperty(
                "Unit",
                typeof(string),
                propertyInfo: typeof(Inventory).GetProperty("Unit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Inventory).GetField("<Unit>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 5);

            var key = runtimeEntityType.AddKey(
                new[] { idpr });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PK__Inventor__B87C5B5F00A47D01");

            var index = runtimeEntityType.AddIndex(
                new[] { itemCode });
            index.AddAnnotation("Relational:Name", "ItemCodeIndex");

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Inventory");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
