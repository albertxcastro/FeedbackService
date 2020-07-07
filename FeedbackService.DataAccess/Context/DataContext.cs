using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FeedbackService.DataAccess.Models;

namespace FeedbackService.DataAccess.Context
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public string ConnectionString { get; set; }

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderToProduct> OrderToProduct { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<User> User { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Sid)
                    .HasName("customer_pkey");

                entity.ToTable("customer", "entity");

                entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .ValueGeneratedNever();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.Sid)
                    .HasName("feedback_pkey");

                entity.ToTable("feedback", "entity");

                entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .ValueGeneratedOnAdd()
                    .UseSerialColumn()
                    .UseIdentityColumn();

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("character varying");

                entity.Property(e => e.CustomerSid).HasColumnName("customer_sid");

                entity.Property(e => e.OrderSid).HasColumnName("order_sid");

                entity.Property(e => e.CreateTime).HasColumnName("create_time");

                entity.Property(e => e.Rating).HasColumnName("rating");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Sid)
                    .HasName("order_pkey");

                entity.ToTable("order", "entity");

                entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasColumnName("create_time");

                entity.Property(e => e.FeedbackSid).HasColumnName("feedback_sid");

                entity.Property(e => e.CustomerSid).HasColumnName("customer_sid");

                entity.Property(e => e.TotalPrice).HasColumnName("total_price");

                entity.Property(e => e.Products).HasColumnName("products");
            });

            modelBuilder.Entity<OrderToProduct>(entity =>
            {
                entity.HasKey(e => new { e.Ordersid, e.ProductSid })
                    .HasName("order_to_product_pkey");

                entity.ToTable("order_to_product", "entity");

                entity.Property(e => e.Ordersid).HasColumnName("ordersid");

                entity.Property(e => e.ProductSid).HasColumnName("product_sid");

                entity.Property(e => e.Ammount)
                    .HasColumnName("ammount")
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.FeedbackSid).HasColumnName("feedback_sid");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Sid)
                    .HasName("product_pkey");

                entity.ToTable("product", "entity");

                entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Sid)
                    .HasName("user_pkey");

                entity.ToTable("user", "security");

                entity.Property(e => e.Sid).HasColumnName("sid");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
