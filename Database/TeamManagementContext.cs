using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using fabsg0.Web.TeamManagement.Blazor.Entities;

namespace fabsg0.Web.TeamManagement.Blazor.Database;

public partial class TeamManagementContext : DbContext
{
    public TeamManagementContext()
    {
    }

    public TeamManagementContext(DbContextOptions<TeamManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentMember> DepartmentMembers { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MembershipFee> MembershipFees { get; set; }

    public virtual DbSet<MembershipFeeDefinition> MembershipFeeDefinitions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=TeamManagement;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<DepartmentMember>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentMembers)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_DepartmentMembers_Departments");

            entity.HasOne(d => d.Member).WithMany(p => p.DepartmentMembers)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_DepartmentMembers_Members");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.HouseNumber).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(100);
        });

        modelBuilder.Entity<MembershipFee>(entity =>
        {
            entity.ToTable("MembershipFee");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

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

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
