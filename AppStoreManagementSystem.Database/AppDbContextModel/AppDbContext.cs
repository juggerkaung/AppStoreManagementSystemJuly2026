using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppStoreManagementSystem.Database.AppDbContextModel;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblApp> TblApps { get; set; }

    public virtual DbSet<TblAppCategory> TblAppCategories { get; set; }

    public virtual DbSet<TblDownload> TblDownloads { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    // => Can't use this if want to use Depedency Injection
//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.; Database=AppStoreMSJuly2026;User Id=sa;Password=sasa@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblApp>(entity =>
        {
            entity.HasKey(e => e.AppId).HasName("PK__Tbl_App__8E2CF7F96B7A8BA8");

            entity.ToTable("Tbl_App");

            entity.HasIndex(e => e.CategoryId, "IX_Tbl_App_CategoryId");

            entity.Property(e => e.AppName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Version).HasMaxLength(20);

            entity.HasOne(d => d.Category).WithMany(p => p.TblApps)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_App_Category");
        });

        modelBuilder.Entity<TblAppCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Tbl_AppC__19093A0B7BCE09EF");

            entity.ToTable("Tbl_AppCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblDownload>(entity =>
        {
            entity.HasKey(e => e.DownloadId).HasName("PK__Tbl_Down__73D5A6F01850BBCF");

            entity.ToTable("Tbl_Download");

            entity.HasIndex(e => e.AppId, "IX_Tbl_Download_AppId");

            entity.HasIndex(e => e.UserId, "IX_Tbl_Download_UserId");

            entity.Property(e => e.DownloadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.App).WithMany(p => p.TblDownloads)
                .HasForeignKey(d => d.AppId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Download_App");

            entity.HasOne(d => d.User).WithMany(p => p.TblDownloads)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Download_User");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Tbl_User__1788CC4CEF9B78D2");

            entity.ToTable("Tbl_User");

            entity.HasIndex(e => e.Email, "UQ__Tbl_User__A9D10534F462D633").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
