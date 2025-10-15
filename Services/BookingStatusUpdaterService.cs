using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShareVolt.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using ShareVolt.Repositories;
using System.Linq;

namespace ShareVolt.Services
{
    public class BookingStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingStatusUpdaterService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

                var bookings = _bookingRepo.GetAllBookings()
                    .Where(b => b.Status == BookingStatus.Confirmed && b.EndTime <= DateTime.Now)
                    .ToList();

                foreach (var b in bookings)
                {
                    b.Status = BookingStatus.Completed;
                    _bookingRepo.UpdateBooking(b);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); 
            }
        }
    }

}
