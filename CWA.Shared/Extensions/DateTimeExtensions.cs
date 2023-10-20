using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static string WeekdayName(this DateTime DateTimeValue, string Language = "es-ES")
        {
            // Force weekday name in a specific culture (Default: Spanish)
            var culture = new CultureInfo(Language);

            return culture.DateTimeFormat.GetDayName(DateTimeValue.DayOfWeek);
        }

        public static string MonthName(this DateTime DateTimeValue, string Language = "es-ES")
        {
            // Force month name in a specific culture (Default: Spanish)
            var culture = new CultureInfo(Language);

            return culture.DateTimeFormat.GetMonthName(DateTimeValue.Month);
        }

        public static string ApplyFormat(this DateTime DateTimeValue, string CustomFormat, string Language = "es-ES")
        {
            // Force formatted date in a specific culture (Default: Spanish)
            var culture = new CultureInfo(Language);
            string result = $"{DateTimeValue.ToShortDateString()} {DateTimeValue.ToShortTimeString()}";

            // If initials requested sent only letters and capitalized
            string initials = $"{DateTimeValue.MonthName()[..1].ToUpper()}{DateTimeValue.MonthName()[1..3]}";
            CustomFormat = CustomFormat.Contains("MMM") ? CustomFormat.Replace("MMM", "XXX") : CustomFormat;

            try { result = DateTimeValue.ToString(CustomFormat, culture).Replace("XXX", initials); } catch { }

            return result;
        }
    }
}
