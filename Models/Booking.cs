using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareVolt.Models
{
    public class Booking
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }


        [DateGreaterThan(nameof(StartTime), ErrorMessage = "End time must be after start time")]
        public DateTime EndTime { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HostEarnings { get; set; }



        [Required(ErrorMessage = "Battery Size is required")]
        public CarBatterySizeType SizeType { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Battery Size must be greater than 0")]
        public int BatterySize { get; set; }





        [Range(0, 99, ErrorMessage = "Battery percentage must be between 0 and 99.")]
        public int ExpectedBatteryPercentage { get; set; }



        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;


        // (Many-to-1) Booking -> User
        public int UserId { get; set; }
        public User User { get; set; }



        // (Many-to-1) Booking -> Charger
        public int ChargerId { get; set; }
        public Charger Charger { get; set; }

        public string PaymentReference { get; set; }
    }

    public enum CarBatterySizeType
    {
        small_BatterySize, // 20 - 40 kw
        meduim_BatterySize, // 40 - 75 kw
        large_BatterySize  // 75 - 180 kw
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public class BatteryMeta
    {
        public CarBatterySizeType SizeType { get; set; }
        public int MinCapacity { get; set; } 
        public int MaxCapacity { get; set; } 
        public int Step { get; set; } = 5;    
    }

    public static class BatteryConfig
    {
        public static readonly List<BatteryMeta> BatteryOptions = new List<BatteryMeta>
        {
            new BatteryMeta { SizeType = CarBatterySizeType.small_BatterySize, MinCapacity = 20, MaxCapacity = 40, Step = 5 },
            new BatteryMeta { SizeType = CarBatterySizeType.meduim_BatterySize, MinCapacity = 40, MaxCapacity = 75, Step = 5 },
            new BatteryMeta { SizeType = CarBatterySizeType.large_BatterySize, MinCapacity = 75, MaxCapacity = 180, Step = 10 }
        };
    }


    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
