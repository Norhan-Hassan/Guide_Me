using Guide_Me.Models;
using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioTranslation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioTranslationController : ControllerBase
    {
        private readonly IAudioTranscriptionService _audioTranscriptionService;
        private readonly ITranslationService _translationService;
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly ILogger<AudioTranslationController> _logger;
        private readonly ITouristService _touristService;
        private readonly ApplicationDbContext _context;

        public AudioTranslationController(
            IAudioTranscriptionService audioTranscriptionService,
            ITranslationService translationService,
            ITextToSpeechService textToSpeechService,
            ILogger<AudioTranslationController> logger,
            ITouristService touristService,
            ApplicationDbContext applicationDbContext)
        {
            _audioTranscriptionService = audioTranscriptionService;
            _translationService = translationService;
            _textToSpeechService = textToSpeechService;
            _logger = logger;
            _touristService = touristService;
            _context = applicationDbContext;
        }

        [HttpPost("translate-audio/{placeName}/{touristName}")]
        public async Task<IActionResult> TranslateAudio(string placeName, string touristName)
        {
            try
            {
                string touristID = _touristService.GetUserIdByUsername(touristName);
                if (touristID != null)
                {
                    string targetLanguage = _context.Tourist.Where(t => t.Id == touristID).FirstOrDefault()?.Language;

                    // Step 1: Transcribe the audio file
                    string transcriptionResult = await _audioTranscriptionService.TranscribeSingleAudioFileAsync(placeName, HttpContext);
                    if (transcriptionResult.StartsWith("Error"))
                    {
                        _logger.LogError($"Error during transcription: {transcriptionResult}");
                        return StatusCode(500, transcriptionResult);
                    }

                    // Extract raw text from transcription result if necessary
                    string transcribedText = ExtractRawText(transcriptionResult);

                    // Step 2: Translate the transcribed text
                    string translationResult = await _translationService.TranslateTextAsync(transcribedText, targetLanguage);
                    if (translationResult == null)
                    {
                        _logger.LogError($"Error during translation: {transcriptionResult}");
                        return StatusCode(500, "Translation error.");
                    }

                    // Extract translated text from translation result if necessary
                    string translatedText = ExtractTranslatedText(translationResult);

                    // Step 3: Convert the translated text to speech
                    string audioPath = await _textToSpeechService.SynthesizeSpeechAsync(translatedText, targetLanguage);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", audioPath);

                    if (System.IO.File.Exists(filePath))
                    {
                        string fileUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{audioPath}";
                        return Ok(new { url = fileUrl });
                    }
                    else
                    {
                        _logger.LogError($"File not found: {filePath}");
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound($"This touristName {touristName} doesn't exist");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing audio translation.");
                return StatusCode(500, "Internal server error");
            }
        }

        private string ExtractRawText(string transcriptionResult)
        {
            // Assuming the transcription result is in the format "Recognized: {text}"
            var prefix = "Recognized: ";
            if (transcriptionResult.StartsWith(prefix))
            {
                return transcriptionResult.Substring(prefix.Length);
            }

            return transcriptionResult;
        }

        private string ExtractTranslatedText(string translationResult)
        {
            // Parse the translation result JSON and extract the translated text
            var translationObject = JsonDocument.Parse(translationResult);
            var translations = translationObject.RootElement[0].GetProperty("translations");
            var translatedText = translations[0].GetProperty("text").GetString();

            return translatedText;
        }
    }
}
