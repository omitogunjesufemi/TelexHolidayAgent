namespace AgentAPI.Models
{
    public class PublicHoliday
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
    }

    public class CountriesAvailable
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
    }

    public class HolidayCache
    {
        private readonly Dictionary<string, List<PublicHoliday>> _holidayCache = new();

        public void CacheHolidays(int year, string countryCode, List<PublicHoliday> holidays)
        {
            _holidayCache[$"{year}_{countryCode}"] = holidays;
        }

        public List<PublicHoliday> GetCachedHolidays(string countryCode, int year)
        {
            return _holidayCache.GetValueOrDefault($"{year}_{countryCode}", new List<PublicHoliday>());
        }
    }
}
