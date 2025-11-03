using A2A;
using AgentAPI.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AgentAPI.Services
{
    public class HolidayAgent
    {
        private readonly ApiClient _holidayAPI;
        private readonly HolidayCache _dataStore = new();
        public HolidayAgent(ApiClient holidayAPI)
        {
            _holidayAPI = holidayAPI;
        }
        public void Attach(ITaskManager taskManager)
        {
            taskManager.OnMessageReceived = ProcessMessageAsync;
            taskManager.OnAgentCardQuery = GetAgentCardAsync;
        }

        private async Task<A2AResponse> ProcessMessageAsync(MessageSendParams messageSendParams, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return await Task.FromCanceled<A2AResponse>(cancellationToken);

            var messageText = messageSendParams.Message.Parts.OfType<TextPart>().FirstOrDefault()?.Text.Trim() ?? string.Empty;
            var contextId = messageSendParams.Message.ContextId ?? string.Empty;

            var match = Regex.Match(messageText, @"^/holiday\s+(next|add)\s+([a-zA-Z]{2})$", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return await Task.FromResult(CreateResponse(contextId, "Invalid command. Try `/holiday next NG` or `/holiday add NG`."));
            }

            Console.WriteLine(match);

            var command = match.Groups[1].Value.ToLower();

            Console.WriteLine(command);
            var countryCode = match.Groups[2].Value.ToUpper();

            Console.WriteLine(countryCode);
            var currentYear = DateTime.Today.Year;
            Console.WriteLine(currentYear);

            if (command == "next")
            {
                if (!_dataStore.GetCachedHolidays(countryCode, currentYear).Any())
                {
                    await FetchAndCacheHolidays(countryCode, currentYear);
                }
            }

            var holidays = _dataStore.GetCachedHolidays(countryCode, currentYear);
            
            string responseText;

            StringBuilder sb = new StringBuilder();
            foreach (var holiday in holidays)
            {
                sb.AppendLine($"Holiday: {holiday.Name}, Date: {holiday.Date}");
            }

            if (sb != null)
            {
                responseText = $"Next Holidays in {GetCountryName(countryCode).Result}\r\n" + sb;
            }
            else
            {
                responseText = "No current holiday for this country";
            }

            var message = CreateResponse(contextId, responseText);

            return await Task.FromResult<A2AResponse>(message);
        }

        private async Task FetchAndCacheHolidays(string countryCode, int year)
        {
            var countryName = GetCountryName(countryCode);
            if ( countryName == null)
                return;

            var response = await _holidayAPI.GetNextPublicHoliday(countryCode);
            var holidays = JsonConvert.DeserializeObject<List<PublicHoliday>>(response);
            holidays = holidays?.Select(holiday => new PublicHoliday
            {
                Date = holiday.Date,
                Name = holiday.Name,
                CountryCode = holiday.CountryCode,
            }).ToList();

            _dataStore.CacheHolidays(year, countryCode, holidays);
        }

        private async Task<string>? GetCountryName(string countryCode)
        {
            var response = await _holidayAPI.GetAvailableCountryAsync();
            var countries = JsonConvert.DeserializeObject<List<CountriesAvailable>>(response);
            countries = countries.Select(c => new CountriesAvailable
            {
                Name = c.Name,
                CountryCode = c.CountryCode
            }).ToList();

            foreach (var country in countries)
            {
                if (country.CountryCode == countryCode.ToUpper())
                    return country.Name;
            }

            return null;
        }

        private Task<AgentCard> GetAgentCardAsync(string agentUrl, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<AgentCard>(cancellationToken);

            var capabilities = new AgentCapabilities()
            {
                Streaming = true,
                PushNotifications = false,
            };

            var skills = new List<AgentSkill>
            {
                new()
                {
                    Name = "/holiday next [Country Code]",
                    Description = "Finds the next public holiday for a country (e.g., NG)"
                }
            };

            return Task.FromResult(new AgentCard()
            {
                Name = "Holiday Agent",
                Description = "The agent ensures teams in different regions are aware of upcoming holidays",
                Url = agentUrl,
                Version = "1.0.0",
                DefaultInputModes = ["text"],
                DefaultOutputModes = ["text"],
                Capabilities = capabilities,
                Skills = skills
            });
        }

        private A2AResponse CreateResponse(string contextId, string text)
        {
            return new AgentMessage
            {
                Role = MessageRole.Agent,
                MessageId = Guid.NewGuid().ToString(),
                ContextId = contextId,
                Parts = [
                    new TextPart { 
                        Text = text 
                    }
                ]
            };
        }
    }
}
