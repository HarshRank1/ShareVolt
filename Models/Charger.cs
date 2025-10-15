using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareVolt.Models
{
    public class Charger
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = "";

        [Required(ErrorMessage = "District is required")]
        public string? District { get; set; } = "";

        [Required(ErrorMessage = "State is required")]
        public string? State { get; set; } = "";

        [Required(ErrorMessage = "Country is required")]
        public string? Country { get; set; } = "";



        [Required(ErrorMessage = "Charger type is required")]
        public ChargerType Type { get; set; }

        [Range(1, 300, ErrorMessage = "Speed must be greater than 0 kW")]
        public int ChargerSpeed { get; set; }


        [Column(TypeName = "decimal(18,4)")]
        [Range(1, double.MaxValue, ErrorMessage = "Price per kWh must be greater than 0")]
        public decimal PricePerKWh { get; set; }



        public bool IsAvailable { get; set; } = true;



        [Url(ErrorMessage = "Please enter a valid Google Maps URL")]
        public string? GoogleMapsUrl { get; set; }

        public int ChargerIncome { get; set; } = 0;


        // (1-to-Many) Charger -> Booking
        public ICollection<Booking> Bookings{ get; set; }



        // (Many-to-1) Charger -> User
        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum ChargerType
    {
        AC1_Slow,
        AC2_Fast,
        DC
    }


    public class ChargerMeta
    {
        public ChargerType Type { get; set; }

        public int MinSpeed { get; set; }
        public int MaxSpeed { get; set; }


        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }

    public static class ChargerConfig
    {
        public static readonly List<ChargerMeta> ChargerOptions = new List<ChargerMeta>
        {
            new ChargerMeta { Type = ChargerType.AC1_Slow,  MinSpeed = 5,  MaxSpeed = 15, MinPrice = 10, MaxPrice = 14 },
            new ChargerMeta { Type = ChargerType.AC2_Fast,  MinSpeed = 15,  MaxSpeed = 50, MinPrice = 14, MaxPrice = 20 },
            new ChargerMeta { Type = ChargerType.DC,  MinSpeed = 50,  MaxSpeed = 150, MinPrice = 18, MaxPrice = 30 }
        };
    }

}
