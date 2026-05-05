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
        //Converts the UTC time to the local time
        public DateTime ConvertToLocal(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            try
            {// Find the configured timezone by its system ID then converts it
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