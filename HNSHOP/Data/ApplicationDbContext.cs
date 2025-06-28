using Microsoft.EntityFrameworkCore;
using HNSHOP.Models;

namespace HNSHOP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<CustomerNotification> CustomerNotifications { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<CustomerTypeSaleEvent> CustomerTypeSaleEvents { get; set; }
        public DbSet<DetailOrder> DetailOrders { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSaleEvent> ProductSaleEvents { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SaleEvent> SaleEvents { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<SubOrder> SubOrders { get; set; }



        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Customer>()
        //        .HasMany(e => e.Notifications)
        //        .WithMany(e => e.Customers)
        //        .UsingEntity<CustomerNotification>();

        //    modelBuilder.Entity<CustomerType>()
        //        .HasMany(e => e.SaleEvents)
        //        .WithMany(e => e.CustomerTypes)
        //        .UsingEntity<CustomerTypeSaleEvent>();

        //    modelBuilder.Entity<Product>()
        //        .HasMany(e => e.SaleEvents)
        //        .WithMany(e => e.Products)
        //        .UsingEntity<ProductSaleEvent>();

        //    modelBuilder.Entity<Order>()
        //        .Property(o => o.Status)
        //        .HasConversion<string>();

        //    modelBuilder.Entity<Order>()
        //        .Property(o => o.PaymentStatus)
        //        .HasConversion<string>();

        //    modelBuilder.Entity<Order>()
        //        .HasOne(o => o.Customer)
        //        .WithMany(c => c.Orders)
        //        .HasForeignKey(o => o.CustomerId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<Like>()
        //        .HasOne(l => l.Product)
        //        .WithMany(p => p.Likes)
        //        .HasForeignKey(l => l.ProductId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<DetailOrder>()
        //        .HasOne(d => d.Product)
        //        .WithMany(p => p.DetailOrders)
        //        .HasForeignKey(d => d.ProductId)
        //        .OnDelete(DeleteBehavior.Restrict);


        //    modelBuilder.Entity<Rating>()
        //        .HasOne(r => r.Customer)
        //        .WithMany(c => c.Ratings)
        //        .HasForeignKey(r => r.CustomerId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<Rating>()
        //        .HasOne(r => r.Product)
        //        .WithMany(p => p.Ratings)
        //        .HasForeignKey(r => r.ProductId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Cấu hình bảng trung gian Notification - Customer
        //    modelBuilder.Entity<CustomerNotification>()
        //        .HasKey(cn => new { cn.CustomerId, cn.NotificationId });

        //    modelBuilder.Entity<CustomerNotification>()
        //        .HasOne(cn => cn.Customer)
        //        .WithMany(c => c.CustomerNotifications)
        //        .HasForeignKey(cn => cn.CustomerId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<CustomerNotification>()
        //        .HasOne(cn => cn.Notification)
        //        .WithMany(n => n.CustomerNotifications)
        //        .HasForeignKey(cn => cn.NotificationId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.SeedAllData();
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Cấu hình bảng trung gian ---
            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Notifications)
                .WithMany(e => e.Customers)
                .UsingEntity<CustomerNotification>();

            modelBuilder.Entity<CustomerType>()
                .HasMany(e => e.SaleEvents)
                .WithMany(e => e.CustomerTypes)
                .UsingEntity<CustomerTypeSaleEvent>();

            modelBuilder.Entity<Product>()
                .HasMany(e => e.SaleEvents)
                .WithMany(e => e.Products)
                .UsingEntity<ProductSaleEvent>();

            // --- Cấu hình Order chính ---
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình SubOrder ---
            modelBuilder.Entity<SubOrder>()
                .HasOne(so => so.Order)
                .WithMany(o => o.SubOrders)
                .HasForeignKey(so => so.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubOrder>()
                .HasOne(so => so.Shop)
                .WithMany(s => s.SubOrders)
                .HasForeignKey(so => so.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình DetailOrder ---
            modelBuilder.Entity<DetailOrder>()
                .HasKey(d => new { d.SubOrderId, d.ProductId });

            modelBuilder.Entity<DetailOrder>()
                .HasOne(d => d.SubOrder)
                .WithMany(so => so.DetailOrders)
                .HasForeignKey(d => d.SubOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetailOrder>()
                .HasOne(d => d.Product)
                .WithMany(p => p.DetailOrders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình Like ---
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Product)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình Rating ---
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình CustomerNotification ---
            modelBuilder.Entity<CustomerNotification>()
                .HasKey(cn => new { cn.CustomerId, cn.NotificationId });

            modelBuilder.Entity<CustomerNotification>()
                .HasOne(cn => cn.Customer)
                .WithMany(c => c.CustomerNotifications)
                .HasForeignKey(cn => cn.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerNotification>()
                .HasOne(cn => cn.Notification)
                .WithMany(n => n.CustomerNotifications)
                .HasForeignKey(cn => cn.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.SeedAllData();

        }




    }
}
