using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.DataDB;

public partial class TeamDropDatabaseContext : DbContext
{
    public TeamDropDatabaseContext()
    {
    }

    public TeamDropDatabaseContext(DbContextOptions<TeamDropDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TestDb> TestDbs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestDb>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("testDb");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Prenume)
                .HasMaxLength(50)
                .HasColumnName("prenume");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
