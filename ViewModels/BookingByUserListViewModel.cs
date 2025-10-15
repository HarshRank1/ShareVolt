using ShareVolt.Models;
using System;

namespace ShareVolt.ViewModels
{
    public class BookingByUserListViewModel
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public string Status { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ChargerId { get; set; }
        public Charger Charger { get; set; }

    }
}
