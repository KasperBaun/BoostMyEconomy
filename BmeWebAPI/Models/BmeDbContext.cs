using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BmeWebAPI.Models
{
    public partial class BmeDbContext : DbContext
    {
        public BmeDbContext()
        {
        }

        public BmeDbContext(DbContextOptions<BmeDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Subcategory> Subcategories { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Transactionitem> Transactionitems { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Decription)
                    .HasMaxLength(200)
                    .HasColumnName("decription");

                entity.Property(e => e.Title)
                    .HasMaxLength(70)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(45)
                    .HasColumnName("description");

                entity.Property(e => e.Title)
                    .HasMaxLength(45)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.ToTable("subcategories");

                entity.HasIndex(e => e.ParentCategoryId, "parentcategoryId_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .HasColumnName("description");

                entity.Property(e => e.ParentCategoryId).HasColumnName("parentCategoryId");

                entity.Property(e => e.Title)
                    .HasMaxLength(70)
                    .HasColumnName("title");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.Subcategories)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("parentCategoryId");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");

                entity.HasIndex(e => e.CategoryId, "categoryId_idx");

                entity.HasIndex(e => e.UserId, "userId_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.Description)
                    .HasMaxLength(400)
                    .HasColumnName("description");

                entity.Property(e => e.MadeAt)
                    .HasMaxLength(100)
                    .HasColumnName("madeAt");

                entity.Property(e => e.Source)
                    .HasMaxLength(100)
                    .HasColumnName("source");

                entity.Property(e => e.SubcategoryId).HasColumnName("subcategoryId");

                entity.Property(e => e.Type)
                    .HasMaxLength(45)
                    .HasColumnName("type");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("categoryId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("userId");
            });

            modelBuilder.Entity<Transactionitem>(entity =>
            {
                entity.ToTable("transactionitems");

                entity.HasIndex(e => e.TransactionId, "TransactionId_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Transactionitems)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TransactionId");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.RoleId, "roleId_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.CreatedAt)
                    .HasMaxLength(100)
                    .HasColumnName("createdAt");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("firstName");

                entity.Property(e => e.Gender)
                    .HasMaxLength(45)
                    .HasColumnName("gender");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("lastName");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(200)
                    .HasColumnName("passwordHash")
                    .HasConversion<string>();

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(200)
                    .HasColumnName("passwordSalt")
                    .HasConversion<string>();

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("roleId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
