using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Ecommerce.Application.Services
{
    public interface IGpsService
    {
        Task<GpsLocationResult> GetLocationAsync(double latitude, double longitude);
        Task<double> GetDistanceAsync(GpsCoordinate from, GpsCoordinate to);
        Task<string> GetAddressFromCoordinatesAsync(double latitude, double longitude);
    }

    public class GpsService :IGpsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GpsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GpsSettings:GoogleMapsApiKey"];
        }


        #region GetLocation
        public async Task<GpsLocationResult> GetLocationAsync(double latitude, double longitude)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var result = doc.RootElement.GetProperty("results")[0];

            return new GpsLocationResult
            {
                FormattedAddress = result.GetProperty("formatted_address").GetString(),
                Latitude = latitude,
                Longitude = longitude
            };
        }
        #endregion

        #region GetDistance
        public async Task<double> GetDistanceAsync(GpsCoordinate from, GpsCoordinate to)
        {
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                      $"?origins={from.Latitude},{from.Longitude}" +
                      $"&destinations={to.Latitude},{to.Longitude}" +
                      $"&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var element = doc.RootElement
                .GetProperty("rows")[0]
                .GetProperty("elements")[0]
                .GetProperty("distance")
                .GetProperty("value"); // meters

            return element.GetDouble() / 1000.0; // convert to km
        }
        #endregion

        #region GetAddressFromCoordinates
        public async Task<string> GetAddressFromCoordinatesAsync(double latitude, double longitude)
        {
            var result = await GetLocationAsync(latitude, longitude);
            return result.FormattedAddress ?? string.Empty;
        }
        #endregion
    }

    public class GpsLocationResult
    {
        public string FormattedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class GpsCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}