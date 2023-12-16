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

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<TestDb> TestDbs { get; set; }

    public virtual DbSet<TransalteFileRef> TransalteFileRefs { get; set; }

    public virtual DbSet<TranslateLanguage> TranslateLanguages { get; set; }

    public virtual DbSet<TranslateLanguagePair> TranslateLanguagePairs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

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

        modelBuilder.Entity<TransalteFileRef>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transalt__3214EC27DE8C14B9");

            entity.ToTable("TransalteFileRef");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.InputFile).HasMaxLength(50);
            entity.Property(e => e.OutputFile).HasMaxLength(50);
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.Target).HasMaxLength(3);
        });

        modelBuilder.Entity<TranslateLanguage>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__Translat__A25C5AA6DF87E7F3");

            entity.ToTable("TranslateLanguage");

            entity.Property(e => e.Code).HasMaxLength(3);
            entity.Property(e => e.NameInternational).HasMaxLength(50);
            entity.Property(e => e.NameLocal).HasMaxLength(50);
        });

        modelBuilder.Entity<TranslateLanguagePair>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Translat__3214EC2793D61D8A");

            entity.ToTable("TranslateLanguagePair");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.Target).HasMaxLength(3);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("idUser");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IdImg).HasColumnName("idImg");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.IdImgNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdImg)
                .HasConstraintName("FK_User_Image");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
