using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCommandDataClient> _logger;
        public HttpCommandDataClient(HttpClient httpClient, ILogger<HttpCommandDataClient> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/c/platforms", httpContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("---> Sync POST to CommandService was OK!");
            }
            else
            {
                _logger.LogInformation("--> Sync POST to CommandService failed");
            }
        }
    }
}