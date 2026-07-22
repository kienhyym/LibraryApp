using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Models;

public partial class LibDbContext : DbContext
{
    public LibDbContext()
    {
    }

    public LibDbContext(DbContextOptions<LibDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrowrecord> Borrowrecords { get; set; }

    public virtual DbSet<Borrowrecorddetail> Borrowrecorddetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Personnel> Personnel { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=LibDB;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("ACCOUNTS");

            entity.HasIndex(e => e.Email, "UQ_ACCOUNTS_Email").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("AUTHORS");

            entity.HasIndex(e => e.AuthorName, "UQ_AUTHORS_AuthorName").IsUnique();

            entity.Property(e => e.AuthorName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nationality).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(255);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("BOOKS");

            entity.Property(e => e.CoverImage).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Publisher).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BOOKS_AUTHORS");

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BOOKS_CATEGORIES");
        });

        modelBuilder.Entity<Borrowrecord>(entity =>
        {
            entity.ToTable("BORROWRECORDS");

            entity.Property(e => e.BorrowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.BorrowRecordStatus).HasDefaultValue(1);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(250);

            entity.HasOne(d => d.Personnel).WithMany(p => p.Borrowrecords)
                .HasForeignKey(d => d.PersonnelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BORROWRECORDS_PERSONNEL");

            entity.HasOne(d => d.Resident).WithMany(p => p.Borrowrecords)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BORROWRECORDS_RESIDENTS");
        });

        modelBuilder.Entity<Borrowrecorddetail>(entity =>
        {
            entity.ToTable("BORROWRECORDDETAILS");

            entity.HasIndex(e => new { e.BorrowRecordId, e.BookId }, "UQ_BORROWRECORDDETAILS_BorrowRecordId_BookId").IsUnique();

            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.ReturnCondition).HasMaxLength(100);
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Borrowrecorddetails)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BORROWRECORDDETAILS_BOOKS");

            entity.HasOne(d => d.BorrowRecord).WithMany(p => p.Borrowrecorddetails)
                .HasForeignKey(d => d.BorrowRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BORROWRECORDDETAILS_BORROWRECORDS");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("CATEGORIES");

            entity.HasIndex(e => e.CategoryName, "UQ_CATEGORIES_CategoryName").IsUnique();

            entity.Property(e => e.CategoryDescription).HasMaxLength(255);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Personnel>(entity =>
        {
            entity.ToTable("PERSONNEL");

            entity.HasIndex(e => e.AccountId, "UQ_PERSONNEL_AccountId").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PersonnelAddress).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Position).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithOne(p => p.Personnel)
                .HasForeignKey<Personnel>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERSONNEL_ACCOUNTS");
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.ToTable("RESIDENTS");

            entity.HasIndex(e => e.AccountId, "UQ_RESIDENTS_AccountId").IsUnique();

            entity.Property(e => e.ApartmentNumber).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PermanentAddress).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithOne(p => p.Resident)
                .HasForeignKey<Resident>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESIDENTS_ACCOUNTS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
