using MemoryCache.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MemoryCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private const string COUNTRIES_KEY = "Countries";

        public CountriesController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            const string restCountriesUrl = "https://restcountries.com/v3.1/all";

            if (_memoryCache.TryGetValue(COUNTRIES_KEY, out List<CountryViewModel> countries))
            {
                return Ok(countries);
            }

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(restCountriesUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Deserialize the JSON into List<CountryInfo>
                    countries = JsonSerializer.Deserialize<List<CountryViewModel>>(responseData, options);

                    // Store in memory cache
                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                        SlidingExpiration = TimeSpan.FromSeconds(1200),
                    };

                    _memoryCache.Set(COUNTRIES_KEY, countries, memoryCacheEntryOptions);

                    return Ok(countries);
                }

                return BadRequest("Failed to fetch data from the API.");
            }
        }
    }
}
