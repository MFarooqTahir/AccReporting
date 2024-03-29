﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using AccReporting.Server;
using AccReporting.Server.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace AccReporting.Server.DbContexts.Configurations
{
    public partial class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> entity)
        {
            entity.HasKey(e => e.Idpr)
                .HasName("PK__Inventor__B87C5B5F00A47D01");

            entity.ToTable("Inventory");

            entity.Property(e => e.Idpr).HasColumnName("IDPr");

            entity.Property(e => e.ItemCode).HasMaxLength(30);

            entity.Property(e => e.ItemDescrip).HasMaxLength(50);

            entity.Property(e => e.ManuName).HasMaxLength(35);

            entity.Property(e => e.MfcCode).HasMaxLength(4);

            entity.Property(e => e.Pressure).HasMaxLength(10);

            entity.Property(e => e.Size).HasMaxLength(15);

            entity.Property(e => e.Unit).HasMaxLength(5);
            entity.HasIndex(e => e.ItemCode).HasDatabaseName("ItemCodeIndex");
            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Inventory> entity);
    }
}
