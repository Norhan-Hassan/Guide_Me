using System.Text.Json.Serialization;

namespace Guide_Me.DTO
{
    public class PlaceWithoutLocationDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int FavoriteFlag { get; set; }
        [JsonPropertyName("media")]
        public List<PlaceMediaDto> Media { get; set; }
    }
}
