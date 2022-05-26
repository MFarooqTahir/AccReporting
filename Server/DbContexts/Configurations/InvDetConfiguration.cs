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
    public partial class InvDetConfiguration : IEntityTypeConfiguration<InvDet>
    {
        public void Configure(EntityTypeBuilder<InvDet> entity)
        {
            entity.HasKey(e => e.Idpr)
                .HasName("PK__InvDet__B87C5B5F67F5802F");

            entity.ToTable("InvDet");

            entity.Property(e => e.Idpr).HasColumnName("IDPr");

            entity.Property(e => e.CateCode).HasMaxLength(4);

            entity.Property(e => e.File)
                .HasMaxLength(10)
                .HasColumnName("FILE");

            entity.Property(e => e.Icode)
                .HasMaxLength(50)
                .HasColumnName("ICode");

            entity.Property(e => e.Iname)
                .HasMaxLength(50)
                .HasColumnName("IName");

            entity.Property(e => e.InvDate).HasColumnType("datetime");

            entity.Property(e => e.PName)
                .HasMaxLength(50)
                .HasColumnName("pName");

            entity.Property(e => e.Pcode)
                .HasMaxLength(50)
                .HasColumnName("PCode");

            entity.Property(e => e.Pressure).HasMaxLength(10);

            entity.Property(e => e.RegionName).HasMaxLength(20);

            entity.Property(e => e.Size).HasMaxLength(15);

            entity.Property(e => e.Sp)
                .HasMaxLength(1)
                .HasColumnName("SP");

            entity.Property(e => e.Type).HasMaxLength(4);

            entity.Property(e => e.Unit).HasMaxLength(5);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<InvDet> entity);
    }
}