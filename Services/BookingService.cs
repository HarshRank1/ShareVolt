using Microsoft.Extensions.Configuration;
using ShareVolt.Models;
using System;

namespace ShareVolt.Services
{
    public class BookingService
    {
        private readonly decimal _hostCommissionRate;
        private readonly decimal _userCommissionRate;

        public BookingService(IConfiguration config)
        {
            _hostCommissionRate = config.GetValue<decimal>("HostCommissionRate");
            _userCommissionRate = config.GetValue<decimal>("UserCommissionRate");
        }

        public void CalculateAmounts(Booking booking, decimal pricePerKwh, decimal unitsConsumed)
        {
            booking.BaseAmount = Math.Round(pricePerKwh * unitsConsumed, 2);
            booking.TotalAmount = Math.Round(booking.BaseAmount + booking.BaseAmount * _userCommissionRate, 2);
            booking.CommissionAmount = Math.Round(booking.BaseAmount * _userCommissionRate + booking.BaseAmount * _hostCommissionRate, 2);
            booking.HostEarnings = Math.Round(booking.BaseAmount - booking.BaseAmount * _hostCommissionRate, 2);
        }

        public DateTime CalculateEndTime(int batteryCapacity, int startPercentage, int chargerSpeedKw, DateTime startDateTime)
        {
            decimal requiredKwh = batteryCapacity * (100 - startPercentage) / 100m;

            decimal requiredHours = requiredKwh / chargerSpeedKw;

            return startDateTime.AddHours((double)requiredHours);
        }


    }
}
