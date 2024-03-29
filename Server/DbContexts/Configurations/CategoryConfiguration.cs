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
    public partial class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasKey(e => e.Idpr)
                .HasName("PK__Category__B87C5B5F9899F9B0");

            entity.ToTable("Category");

            entity.Property(e => e.Idpr).HasColumnName("IDPr");

            entity.Property(e => e.CategoryName).HasMaxLength(35);

            entity.Property(e => e.Code).HasMaxLength(4);

            entity.Property(e => e.Color).HasMaxLength(100);

            entity.Property(e => e.Date1).HasColumnType("datetime");

            entity.Property(e => e.Item).HasMaxLength(100);

            entity.Property(e => e.JoiningType).HasMaxLength(50);

            entity.Property(e => e.Origin).HasMaxLength(100);

            entity.Property(e => e.Standard).HasMaxLength(100);

            entity.Property(e => e.Temp4).HasMaxLength(50);

            entity.Property(e => e.Temp5).HasMaxLength(50);

            entity.Property(e => e.Temp6).HasMaxLength(50);

            entity.Property(e => e.Temp7).HasMaxLength(50);

            entity.Property(e => e.UserName).HasMaxLength(10);
            entity.HasIndex(e => e.Code).HasDatabaseName("CodeIndex");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Category> entity);
    }
}
