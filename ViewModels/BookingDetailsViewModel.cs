using System;

namespace ShareVolt.ViewModels
{
    public class BookingDetailsViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string SizeType { get; set; }
        public int BatterySize { get; set; }
        public int ExpectedBatteryPercentage { get; set; }

        public string Status { get; set; }

        // User who is charging
        public string UserEmail { get; set; }

        // Host (charger owner)
        public string HostEmail { get; set; }
    }
}
