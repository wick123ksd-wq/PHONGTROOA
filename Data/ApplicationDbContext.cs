using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Models;

namespace PHONGTROOA.Data
{
    public class ApplicationDbContext
        : IdentityDbContext   // 🔥 เปลี่ยนตรงนี้
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Phong> Phongs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; } // 🔥 ต้องมี
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Phong>()
       .Property(p => p.GiaThue)
       .HasPrecision(18, 2);

            // Booking → Phong
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Phong)
                .WithMany()
                .HasForeignKey(b => b.PhongId)
                .HasPrincipalKey(p => p.MaPhong);

            // Booking money
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            // Payment
            modelBuilder.Entity<Payment>()
    .HasOne(p => p.Booking)
    .WithOne(b => b.Payment)
    .HasForeignKey<Payment>(p => p.BookingId);

            // Cart
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);
        }
    }
}