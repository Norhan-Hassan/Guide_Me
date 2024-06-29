using Guide_Me.Services;
using RestSharp;
using System.Threading.Tasks;
namespace Guide_Me.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly string _endpoint;
        private readonly string _SubscriptionKey;
        private readonly string _region;
        private readonly ITouristService _touristService;

        public TranslationService(IConfiguration configuration)
        {
            _endpoint = configuration["TranslatorText:Endpoint"];
            _SubscriptionKey = configuration["TranslatorText:SubscriptionKey"];
            _region = configuration["TranslatorText:Region"];
            // Add debug output
            Console.WriteLine($"Endpoint: {_endpoint}, SubscriptionKey: {_SubscriptionKey}, Region: {_region}");
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {

            var client = new RestClient(_endpoint);
            var request = new RestRequest("/translate?api-version=3.0&to=" + targetLanguage, Method.Post);
            request.AddHeader("Ocp-Apim-Subscription-Key", _SubscriptionKey);
            request.AddHeader("Ocp-Apim-Subscription-Region", _region);
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new object[] { new { Text = text } });
            var response = await client.ExecuteAsync(request);
            return response.Content;
        }
    }
}