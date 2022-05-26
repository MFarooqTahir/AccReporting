﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace AccReporting.Server.OptimizedModels
{
    public partial class AccountInfoDbContextModel
    {
        partial void Initialize()
        {
            var acfile = AcfileEntityType.Create(this);
            var basic = BasicEntityType.Create(this);
            var category = CategoryEntityType.Create(this);
            var invDet = InvDetEntityType.Create(this);
            var inventory = InventoryEntityType.Create(this);
            var invSumm = InvSummEntityType.Create(this);
            var trans = TransEntityType.Create(this);

            AcfileEntityType.CreateAnnotations(acfile);
            BasicEntityType.CreateAnnotations(basic);
            CategoryEntityType.CreateAnnotations(category);
            InvDetEntityType.CreateAnnotations(invDet);
            InventoryEntityType.CreateAnnotations(inventory);
            InvSummEntityType.CreateAnnotations(invSumm);
            TransEntityType.CreateAnnotations(trans);

            AddAnnotation("ProductVersion", "6.0.5");
            AddAnnotation("Relational:MaxIdentifierLength", 128);
            AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}