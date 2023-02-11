using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineSchoolMVCWebApp.Models;

public partial class OnlineSchoolDbContext : DbContext
{
    public OnlineSchoolDbContext()
    {
    }

    public OnlineSchoolDbContext(DbContextOptions<OnlineSchoolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Cource> Cources { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<SubjectCategory> SubjectCategories { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=OnlineSchoolDb; Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Books");

            entity.Property(e => e.Link).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Cource).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.CourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachments_Cources");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(20);
        });

        modelBuilder.Entity<Cource>(entity =>
        {
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Cources)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cources_Authors");

            entity.HasOne(d => d.Level).WithMany(p => p.Cources)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cources_Levels");

            entity.HasOne(d => d.SubjectCategory).WithMany(p => p.Cources)
                .HasForeignKey(d => d.SubjectCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cources_SubjectCategories");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<SubjectCategory>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Cource).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Cources");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
