using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareVolt.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = "";


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = "";



        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6–100 characters")]
        public string Password { get; set; } = "";


        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Wallet balance cannot be negative")]
        public decimal WalletBalance { get; set; } = 0;

        public decimal TurnOver { get; set; } = 0;


        public bool IsAdmin { get; set; } = false;

        public ICollection<Charger> Chargers { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }

}
