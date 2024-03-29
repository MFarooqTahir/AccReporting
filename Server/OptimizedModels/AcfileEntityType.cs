﻿// <auto-generated />
using System;
using System.Reflection;
using AccReporting.Shared.ContextModels;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace AccReporting.Server.OptimizedModels
{
    internal partial class AcfileEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "AccReporting.Shared.ContextModels.Acfile",
                typeof(Acfile),
                baseEntityType);

            var idpr = runtimeEntityType.AddProperty(
                "Idpr",
                typeof(int),
                propertyInfo: typeof(Acfile).GetProperty("Idpr", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Idpr>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);
            idpr.AddAnnotation("Relational:ColumnName", "IDPr");

            var actCode = runtimeEntityType.AddProperty(
                "ActCode",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("ActCode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<ActCode>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 30);

            var actName = runtimeEntityType.AddProperty(
                "ActName",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("ActName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<ActName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var address1 = runtimeEntityType.AddProperty(
                "Address1",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Address1", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Address1>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var address2 = runtimeEntityType.AddProperty(
                "Address2",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Address2", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Address2>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var address3 = runtimeEntityType.AddProperty(
                "Address3",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Address3", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Address3>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var crDays = runtimeEntityType.AddProperty(
                "CrDays",
                typeof(int?),
                propertyInfo: typeof(Acfile).GetProperty("CrDays", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<CrDays>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var email = runtimeEntityType.AddProperty(
                "Email",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Email", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Email>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 40);
            email.AddAnnotation("Relational:ColumnName", "email");

            var fax = runtimeEntityType.AddProperty(
                "Fax",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Fax", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Fax>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20);
            fax.AddAnnotation("Relational:ColumnName", "fax");

            var gst = runtimeEntityType.AddProperty(
                "Gst",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Gst", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Gst>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 100);
            gst.AddAnnotation("Relational:ColumnName", "GST");

            var opBal = runtimeEntityType.AddProperty(
                "OpBal",
                typeof(double?),
                propertyInfo: typeof(Acfile).GetProperty("OpBal", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<OpBal>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var phone = runtimeEntityType.AddProperty(
                "Phone",
                typeof(string),
                propertyInfo: typeof(Acfile).GetProperty("Phone", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Acfile).GetField("<Phone>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20);
            phone.AddAnnotation("Relational:ColumnName", "phone");

            var key = runtimeEntityType.AddKey(
                new[] { idpr });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PK__Acfile__B87C5B5F3831C82B");

            var index = runtimeEntityType.AddIndex(
                new[] { actCode });
            index.AddAnnotation("Relational:Name", "ActCodeIndex");

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Acfile");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
