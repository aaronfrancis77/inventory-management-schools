namespace DaymapInventory.Helpers
{
    public class DateTimeHelper
    {
        private readonly string _targetTimeZoneId;

        public DateTimeHelper(IConfiguration configuration)
        {
            // Fallback to UTC if no env variable is set
            _targetTimeZoneId = configuration["InventorySettings:LocalTimeZone"] ?? "UTC";
            
        }

        
        public DateTime ConvertToLocal(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            try
            {
                var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_targetTimeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, targetTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                return utcDateTime; // Fallback to UTC if ID is invalid
            }
        }
    }
}