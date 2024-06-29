using System.Text.Json.Serialization;

namespace Guide_Me.DTO
{
    public class PlaceDto
    {
        
        public string Name { get; set; }
        public string Category { get; set; }
        public int FavoriteFlag { get; set; }
        [JsonPropertyName("media")]
        public List<PlaceMediaDto> Media { get; set; }
        
    }
}
