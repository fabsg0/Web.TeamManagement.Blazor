using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using fabsg0.Web.TeamManagement.Blazor.Entities;

namespace fabsg0.Web.TeamManagement.Blazor.Database;

public partial class TeamManagementContext(DbContextOptions<TeamManagementContext> options) : DbContext(options)
{
    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentMember> DepartmentMembers { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MembershipFee> MembershipFees { get; set; }

    public virtual DbSet<MembershipFeeDefinition> MembershipFeeDefinitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.HasPostgresEnum<Sex>();

        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<DepartmentMember>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "IX_DepartmentMembers_DepartmentId");

            entity.HasIndex(e => e.MemberId, "IX_DepartmentMembers_MemberId");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentMembers)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_DepartmentMembers_Departments");

            entity.HasOne(d => d.Member).WithMany(p => p.DepartmentMembers)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_DepartmentMembers_Members");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.HouseNumber).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Sex)
                .HasColumnType("sex_enum")
                .IsRequired();;
            entity.Property(e => e.Street).HasMaxLength(100);
            entity.Property(e => e.Telephone).HasMaxLength(50);
        });

        modelBuilder.Entity<MembershipFee>(entity =>
        {
            entity.ToTable("MembershipFee");

            entity.HasIndex(e => e.MemberId, "IX_MembershipFee_MemberId");

            entity.HasIndex(e => e.MemberhsipFeeDefinitionId, "IX_MembershipFee_MemberhsipFeeDefinitionId");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Member).WithMany(p => p.MembershipFees)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_MembershipFee_Members");

            entity.HasOne(d => d.MemberhsipFeeDefinition).WithMany(p => p.MembershipFees)
                .HasForeignKey(d => d.MemberhsipFeeDefinitionId)
                .HasConstraintName("FK_MembershipFee_MembershipFeeDefinition");
        });

        modelBuilder.Entity<MembershipFeeDefinition>(entity =>
        {
            entity.ToTable("MembershipFeeDefinition");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
