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
        public DbSet<UserNotification> UserNotifications { get; set; }
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
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Cấu hình bảng trung gian ---
           

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
            .OnDelete(DeleteBehavior.Restrict); // Hoặc .NoAction nếu tránh vòng lặp

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
             .HasOne(r => r.SubOrder)
             .WithMany()
             .HasForeignKey(r => r.SubOrderId)
             .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<UserNotification>()
     .HasKey(un => new { un.AccountId, un.NotificationId });

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.Account)
                .WithMany(a => a.UserNotifications)
                .HasForeignKey(un => un.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);




            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Shop)
                .WithMany()
                .HasForeignKey(c => c.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);


        }




    }
}
