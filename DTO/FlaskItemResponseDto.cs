using System.Text.Json.Serialization;

namespace Guide_Me.DTO
{
    public class FlaskItemResponseDto
    {
        [JsonPropertyName("place_name")]
        public string PlaceName { get; set; }
        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("similar_item")]
        public string SimilarItem { get; set; }
    }
}
