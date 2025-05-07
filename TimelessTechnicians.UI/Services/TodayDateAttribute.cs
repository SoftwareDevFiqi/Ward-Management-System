using System.ComponentModel.DataAnnotations;

namespace TimelessTechnicians.UI.Services
{
    public class TodayDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime dateTimeValue)
            {
                // Check if the date is today
                return dateTimeValue.Date == DateTime.Today;
            }
            return true; // If not a DateTime, it's valid
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be today's date.";
        }
    }
}
