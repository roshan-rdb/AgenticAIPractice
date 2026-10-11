using System.ComponentModel;

namespace AgenticAIPractice
{
    public static class DateTimeTools
    {
        [Description("Returns the current date and time. Use when the user asks about current time or date.")]
        public static string GetCurrentDateTimeString(
            [Description("The type of time: 'local' for local time, or 'utc' for UTC")]
            string timeType= "local"
            )
        {
            var dateTime = timeType.ToLower() switch
            {
                "local" => DateTime.Now,
                "utc" => DateTime.UtcNow,
                _ => throw new ArgumentException("Invalid TimeType. Use 'local' or 'utc'.")
            };
            return $"{dateTime:dddd, MMMM dd, yyyy - HH:mm:ss} ({timeType.ToUpperInvariant()})";
        }


        [Description("Calculates days between two dates. Use when the user asks how much time until a date.")]
        public static string CalculateDateDifference(
        [Description("First date in yyyy-MM-dd format (e.g., 2026-12-25)")]
        string fromDate,
        [Description("Second date in yyyy-MM-dd format. If omitted, uses today.")]
        string? toDate = null)
        {
            try
            {
                var from = DateTime.Parse(fromDate);
                var to = toDate != null ? DateTime.Parse(toDate) : DateTime.Now;
                var diff = to - from;
                var direction = diff.TotalDays >= 0 ? "remaining" : "have passed";
                var absDiff = diff.TotalDays >= 0 ? diff : -diff;
                return $"From {from:MM/dd/yyyy} to {to:MM/dd/yyyy}: " +
                       $"{(int)absDiff.TotalDays} days {direction}";
            }
            catch (FormatException)
            {
                return "Error: invalid date format. Use yyyy-MM-dd (e.g., 2026-12-25)";
            }
        }
    }
}
