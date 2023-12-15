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

    public virtual DbSet<TranslateLanguage> TranslateLanguages { get; set; }

    public virtual DbSet<TranslateLanguagePair> TranslateLanguagePairs { get; set; }

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

        modelBuilder.Entity<TranslateLanguage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TranslateLanguage");

            entity.Property(e => e.Code).HasMaxLength(3);
            entity.Property(e => e.NameInternational).HasMaxLength(50);
            entity.Property(e => e.NameLocal).HasMaxLength(50);
        });

        modelBuilder.Entity<TranslateLanguagePair>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TranslateLanguagePair");

            entity.Property(e => e.Source).HasMaxLength(3);
            entity.Property(e => e.Target).HasMaxLength(3);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
