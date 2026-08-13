using System;
using System.Collections.Generic;
using LibraryApp.Enums;
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

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Favoritebook> Favoritebooks { get; set; }

    public virtual DbSet<Personnel> Personnel { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=LibDB;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True");

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

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BOOKS_CATEGORIES");

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "Bookauthor",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("FK_BOOKAUTHORS_AUTHORS"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK_BOOKAUTHORS_BOOKS"),
                    j =>
                    {
                        j.HasKey("BookId", "AuthorId");
                        j.ToTable("BOOKAUTHORS");
                        j.HasIndex(new[] { "AuthorId" }, "IX_BOOKAUTHORS_AuthorId");
                    });
        });

        modelBuilder.Entity<Borrowrecord>(entity =>
        {
            entity.ToTable("BORROWRECORDS");

            entity.Property(e => e.BorrowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.BorrowRecordStatus).HasDefaultValue(BorrowRecordStatus.Borrowing);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(250);
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");

            entity.HasOne(d => d.Personnel).WithMany(p => p.BorrowrecordPersonnel)
                .HasForeignKey(d => d.PersonnelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BORROWRECORDS_PERSONNEL");

            entity.HasOne(d => d.Resident).WithMany(p => p.Borrowrecords)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BORROWRECORDS_RESIDENTS");

            entity.HasOne(d => d.ReturnPersonnel).WithMany(p => p.BorrowrecordReturnPersonnel)
                .HasForeignKey(d => d.ReturnPersonnelId)
                .HasConstraintName("FK_BORROWRECORDS_RETURNPERSONNEL");
        });

        modelBuilder.Entity<Borrowrecorddetail>(entity =>
        {
            entity.ToTable("BORROWRECORDDETAILS");

            entity.HasIndex(e => new { e.BorrowRecordId, e.BookId }, "UQ_BORROWRECORDDETAILS_BorrowRecordId_BookId").IsUnique();

            entity.Property(e => e.Penalty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReturnNote).HasMaxLength(500);

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

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.ToTable("EMAIL_VERIFICATIONS");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Favoritebook>(entity =>
        {
            entity.ToTable("FAVORITEBOOK");

            entity.HasIndex(e => new { e.ResidentId, e.BookId }, "UQ_FAVORITEBOOK").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Favoritebooks)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FAVORITEBOOK_BOOK");

            entity.HasOne(d => d.Resident).WithMany(p => p.Favoritebooks)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FAVORITEBOOK_RESIDENT");
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
