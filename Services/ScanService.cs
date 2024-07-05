
namespace Guide_Me.Services
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Guide_Me.DTO;
    using Guide_Me.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ScanService : IScanService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IPlaceService _placeService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly string _flaskBaseUrl;

        public ScanService(IHttpClientFactory httpClientFactory, 
            IConfiguration configuration,
            ApplicationDbContext context,
            IPlaceService placeService,
            IBlobStorageService blobStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
            _placeService = placeService;

            // Retrieve base URL from appsettings.json
            _flaskBaseUrl = _configuration["Flask:BaseUrl"];
            _blobStorageService = blobStorageService;
        }

        public async Task<ScannedImageResultDto> GetSimilarPlacesAsync(IFormFile image, string cityName)
        {
            var endpoint = $"{_flaskBaseUrl}/scan_place";
            var flaskResponse = await PostToFlaskApiAsync<FlaskResponseDto>(image, cityName, endpoint);

            if (flaskResponse == null)
            {
                Console.WriteLine("Deserialization failed.");
                return new ScannedImageResultDto { CityName = cityName, SimilarPlaceData = new List<SimilarPlaceDataDto>() };
            }

            Console.WriteLine($"CityName: {flaskResponse.CityName ?? "null"}");
            Console.WriteLine($"Filename: {flaskResponse.Filename ?? "null"}");
            Console.WriteLine($"Message: {flaskResponse.Message ?? "null"}");

            var scannedImageResult = new ScannedImageResultDto
            {
                CityName = flaskResponse.CityName,
                SimilarPlaceData = await GetSimilarPlacesFromDbAsync(flaskResponse.SimilarPlaces)
            };

            return scannedImageResult;
        }

        public async Task<ScannedItemResultDto> GetSimilarItemsAsync(IFormFile itemImage, string placeName)
        {
            var endpoint = $"{_flaskBaseUrl}/scan_item";
            var flaskResponse = await PostToFlaskApiAsync<FlaskItemResponseDto>(itemImage, placeName, endpoint);

            if (flaskResponse == null)
            {
                Console.WriteLine("Deserialization failed.");
                return new ScannedItemResultDto { PlaceName = placeName, SimilarItemData = null };
            }

            Console.WriteLine($"PlaceName: {flaskResponse.PlaceName ?? "null"}");
            Console.WriteLine($"Filename: {flaskResponse.Filename ?? "null"}");
            Console.WriteLine($"Message: {flaskResponse.Message ?? "null"}");

            var scannedItemResult = new ScannedItemResultDto
            {
                PlaceName = flaskResponse.PlaceName,
                SimilarItemData = await GetSimilarItemFromDbAsync(flaskResponse.SimilarItem)
            };

            return scannedItemResult;
        }

        private async Task<T> PostToFlaskApiAsync<T>(IFormFile image, string parameter, string url)
        {
            using (var content = new MultipartFormDataContent())
            {
                using (var ms = new MemoryStream())
                {
                    await image.CopyToAsync(ms);
                    var fileContent = new ByteArrayContent(ms.ToArray());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(image.ContentType);
                    content.Add(fileContent, "file", image.FileName); // 'file' is the form field name for the file
                }

                // Determine parameter name based on endpoint
                var paramName = url.Contains("scan_place") ? "city" : "place";
                content.Add(new StringContent(parameter), paramName);

                var httpClient = _httpClientFactory.CreateClient();

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode(); // Ensure the request succeeds (status code 2xx)

                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Raw response from Flask API: " + responseString);

                    return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Request Exception: {ex.Message}");
                    throw; // Rethrow the exception to handle it in your controller or application layer
                }
            }
        }

        public async Task<List<SimilarPlaceDataDto>> GetSimilarPlacesFromDbAsync(List<string> similarPlaceNames)
        {
            var result = new List<SimilarPlaceDataDto>();
            foreach (var similarPlaceName in similarPlaceNames)
            {
                var place = _context.Places.FirstOrDefault(p => p.PlaceName == similarPlaceName);
                if (place != null)
                {
                    var media = await _context.placeMedias.FirstOrDefaultAsync(p => p.PlaceId == place.Id && p.MediaType.ToLower() == "image");
                    var similarPlaceDto = new SimilarPlaceDataDto
                    {
                        PlaceName = place.PlaceName,
                        Image = _blobStorageService.GetBlobUrlmedia(media.MediaContent)
                    };
                    result.Add(similarPlaceDto);
                }
            }
            return await Task.FromResult(result);
        }

        public async Task<SimilarItemDataDto> GetSimilarItemFromDbAsync(string similarItemName)
        {
            if (string.IsNullOrEmpty(similarItemName))
            {
                return null;
            }

            var item = _context.placeItem.FirstOrDefault(i => i.placeItemName == similarItemName);
            if (item != null)
            {
                var media = await _context.placeItemMedias.FirstOrDefaultAsync(i => i.placeItemID == item.ID && i.MediaType == "audio");
                var similarItemDto = new SimilarItemDataDto
                {
                    ItemName = item.placeItemName,
                    Image =  _blobStorageService.GetBlobUrlmedia(media.MediaContent)
                };
                return await Task.FromResult(similarItemDto);
            }

            return null;
        }
    }
}
