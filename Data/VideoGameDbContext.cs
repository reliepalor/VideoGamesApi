using Microsoft.EntityFrameworkCore;
using VideoGameApi.Models.Carts;
using VideoGameApi.Models.Conversations;
using VideoGameApi.Models.DigitalOrders;
using VideoGameApi.Models.DigitalProducts;
using VideoGameApi.Models.Games;
using VideoGameApi.Models.Orders;
using VideoGameApi.Models.Reviews;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Data
{

    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();

        public DbSet<User> Users { get; internal set; } = null!;
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMessage> ConversationMessages { get; set; }

        public DbSet<DigitalProduct> DigitalProducts => Set<DigitalProduct>();
        public DbSet<DigitalProductKey> DigitalProductKeys => Set<DigitalProductKey>();

        public DbSet<DigitalOrder> DigitalOrders => Set<DigitalOrder>();
        public DbSet<DigitalOrderItem> DigitalOrderItems => Set<DigitalOrderItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VideoGame>()
                .Property(v => v.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Order)
                .WithMany()
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.VideoGameId, r.OrderId })
                .IsUnique();

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConversationMessage>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<ConversationMessage>()
                .HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DigitalOrder>()
                .HasOne(o => o.DigitalProduct)
                .WithMany()
                .HasForeignKey(o => o.DigitalProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DigitalOrder>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DigitalOrder>()
                .HasMany(o => o.Items)
                .WithOne(i => i.DigitalOrder)
                .HasForeignKey(i => i.DigitalOrderId);

            modelBuilder.Entity<DigitalOrderItem>()
                .HasOne(i => i.DigitalProductKey)
                .WithMany()
                .HasForeignKey(i => i.DigitalProductKeyId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}

    