using ShareVolt.Models;
using System.Collections;
using System.Collections.Generic;

namespace ShareVolt.Repositories
{
    public interface IBookingRepository
    {
        Booking GetBooking(int id);
        IEnumerable<Booking> GetAllBookings();
        Booking AddBooking(Booking booking);
        Booking UpdateBooking(Booking booking);
        Booking DeleteBooking(int id);
        IEnumerable<Booking> GetBookingByUserId(int id);
        IEnumerable<Booking> GetBookingByChargerId(int id);

    }
}
