using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using ShareVolt.ViewModels;

namespace ShareVolt.Models
{
    public class ShareVoltDbContext : DbContext
    {
        public ShareVoltDbContext(DbContextOptions<ShareVoltDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Charger> Chargers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
             .HasOne(b => b.User)
             .WithMany(u => u.Bookings)
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
              .HasOne(b => b.Charger)
              .WithMany(c => c.Bookings)
              .HasForeignKey(b => b.ChargerId)
              .OnDelete(DeleteBehavior.Cascade);

        }

        //public DbSet<ShareVolt.ViewModels.EditUserViewModel> EditUserViewModel { get; set; }

        //public DbSet<ShareVolt.ViewModels.BookingCreateViewModel> BookingCreateViewModel { get; set; }

        //public DbSet<ShareVolt.ViewModels.BookingDetailsViewModel> BookingDetailsViewModel { get; set; }

        //public DbSet<ShareVolt.ViewModels.BookingByUserListViewModel> BookingByUserListViewModel { get; set; }

        //public DbSet<ShareVolt.ViewModels.BookingByChargerListViewModel> BookingByChargerDetailsViewModel { get; set; }
    }
}
