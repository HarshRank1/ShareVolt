using Microsoft.EntityFrameworkCore;
using ShareVolt.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShareVolt.Repositories
{
    public class SQLBookingRepository : IBookingRepository
    {
        private readonly ShareVoltDbContext _context;
        public SQLBookingRepository(ShareVoltDbContext context)
        {
            _context = context;
        }

        public Booking AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return booking;
        }

        public Booking DeleteBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
            return booking;
        }


        public IEnumerable<Booking> GetAllBookings()
        {
            return _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Charger)
                    .ThenInclude(c => c.User)
                .ToList();
        }

        public Booking GetBooking(int id)
        {
            return _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Charger)
                    .ThenInclude(c => c.User)
                .FirstOrDefault(b => b.Id == id);
        }

        public Booking UpdateBooking(Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
            return booking;
        }

        public IEnumerable<Booking> GetBookingByUserId(int id)
        {
            return _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Charger)
                    .ThenInclude(c => c.User)
                .Where(b => b.UserId == id)
                .ToList();
        }

        public IEnumerable<Booking> GetBookingByChargerId(int id)
        {
            return _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Charger)
                    .ThenInclude(c => c.User)
                .Where(b => b.ChargerId == id)
                .ToList();
        }
    }
}
