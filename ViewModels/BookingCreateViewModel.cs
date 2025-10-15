using ShareVolt.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareVolt.ViewModels
{
    public class BookingCreateViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        
        
        [Required(ErrorMessage = "Start time is required")]
        public TimeSpan StartTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var now = DateTime.Now;

            // Combine selected Date and StartTime
            var selectedDateTime = Date.Date.Add(StartTime);

            if (selectedDateTime < now)
            {
                yield return new ValidationResult(
                    "Start time cannot be in the past.",
                    new[] { nameof(StartTime) });
            }
        }


        [Required(ErrorMessage = "Battery Size is required")]
        public CarBatterySizeType SizeType { get; set; }

        
        
        [Required(ErrorMessage = "Please select battery capacity")]
        [Range(1, int.MaxValue, ErrorMessage = "Battery Size must be greater than 0")]
        public int BatteryCapacity { get; set; }



        [Range(0, 99, ErrorMessage = "Battery percentage must be between 0 and 99.")]
        public int ExpectedBatteryPercentage { get; set; }

        
        
        [Required]
        public int ChargerId { get; set; }
        public decimal PricePerKwh { get; set; }

        [NotMapped]
        public List<BatteryMeta> BatteryOptions { get; set; }
    }

}
