using System.Text.Json.Serialization;
namespace Guide_Me.DTO
{
  
public class FlaskResponseDto
    {
        [JsonPropertyName("city_name")]
        public string CityName { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("similar_places")]
        public List<string> SimilarPlaces { get; set; }
    }
}

