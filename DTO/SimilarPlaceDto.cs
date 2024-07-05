
using System.Text.Json.Serialization;

namespace Guide_Me.DTO
{
    public class SimilarPlaceDto
    {
        [JsonPropertyName("place_name")]
        public string PlaceName { get; set; }

        [JsonPropertyName("similarity_score")]
        public double SimilarityScore { get; set; }

    }
}
