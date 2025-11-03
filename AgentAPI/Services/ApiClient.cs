namespace AgentAPI.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAvailableCountryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/v3/AvailableCountries");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException hEx)
            {
                return $"Error fetching cat fact: {hEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Error fetching cat fact: {ex.Message}";
            }
        }

        public async Task<string> GetNextPublicHoliday(string countryCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v3/NextPublicHolidays/{countryCode}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException hEx)
            {
                return $"Error fetching cat fact: {hEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Error fetching cat fact: {ex.Message}";
            }
        }

        public async Task<string> GetPublicHoliday(int year, string countryCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v3/PublicHolidays/{year}/{countryCode}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException hEx)
            {
                return $"Error fetching cat fact: {hEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Error fetching cat fact: {ex.Message}";
            }
        }
    }
}



