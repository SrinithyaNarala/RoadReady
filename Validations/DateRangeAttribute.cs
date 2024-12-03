using System.ComponentModel.DataAnnotations;

namespace RoadReady.Validations
{
    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateRangeAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentDate = (DateTime)value;

            // Get the comparison property (e.g., PickupDate)
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            var comparisonDate = (DateTime)property.GetValue(validationContext.ObjectInstance);

            // Check if Drop-off Date is later than Pickup Date
            if (currentDate <= comparisonDate)
            {
                return new ValidationResult("Drop-off date must be later than pickup date.");
            }

            return ValidationResult.Success;
        }
    }
}
