using Guide_Me.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Guide_Me.Services
{
    public interface IScanService
    {
            Task<ScannedImageResultDto> GetSimilarPlacesAsync(IFormFile image, string cityName);
           Task<ScannedItemResultDto> GetSimilarItemsAsync(IFormFile image, string cityName);

    }
}
