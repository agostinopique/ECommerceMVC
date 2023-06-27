using System;
using System.Collections.Generic;
using ECommerceMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Data;

public partial class EcommerceApiContext : DbContext
{
    public EcommerceApiContext()
    {
    }

    public EcommerceApiContext(DbContextOptions<EcommerceApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ErrorLog> Errors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source =localhost; Initial Catalog=ECommerceAPI; TrustServerCertificate=True; Integrated Security=True; Persist Security Info=False;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasMany(d => d.Products).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderProduct",
                    r => r.HasOne<Product>().WithMany().HasForeignKey("ProductsId"),
                    l => l.HasOne<Order>().WithMany().HasForeignKey("OrdersId"),
                    j =>
                    {
                        j.HasKey("OrdersId", "ProductsId");
                        j.ToTable("OrderProduct");
                        j.HasIndex(new[] { "ProductsId" }, "IX_OrderProduct_ProductsId");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
