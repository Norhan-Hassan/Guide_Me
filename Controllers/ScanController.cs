using Guide_Me.DTO;
using Guide_Me.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ScanController : ControllerBase
{
    private readonly IScanService _scanService;

    public ScanController(IScanService scanService)
    {
        _scanService = scanService;
    }

    [HttpPost("scan-place")]
    public async Task<IActionResult> UploadImage([FromForm] imageUploadDto imageUploadDto)
    {
        if (imageUploadDto.Image == null || imageUploadDto.Image.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (string.IsNullOrEmpty(imageUploadDto.CityName))
        {
            return BadRequest("City name is required.");
        }

        var similarPlaces = await _scanService.GetSimilarPlacesAsync(imageUploadDto.Image, imageUploadDto.CityName);
        return Ok(similarPlaces);
    }
    [HttpPost("scan-item")]
    public async Task<IActionResult> ScanItem(IFormFile itemImage, [FromForm] string placeName)
    {
        if (itemImage == null || string.IsNullOrEmpty(placeName))
        {
            return BadRequest("Item image or place name is missing");
        }

        var result = await _scanService.GetSimilarItemsAsync(itemImage, placeName);
        return Ok(result);
    }
}
