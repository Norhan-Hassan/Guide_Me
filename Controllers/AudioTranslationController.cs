using Guide_Me.Models;
using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        private readonly IBlobStorageService _blobStorageService;

        public AudioTranslationController(
            IAudioTranscriptionService audioTranscriptionService,
            ITranslationService translationService,
            ITextToSpeechService textToSpeechService,
            ILogger<AudioTranslationController> logger,
            ITouristService touristService,
            ApplicationDbContext applicationDbContext,
            IBlobStorageService blobStorageService)
        {
            _audioTranscriptionService = audioTranscriptionService;
            _translationService = translationService;
            _textToSpeechService = textToSpeechService;
            _logger = logger;
            _touristService = touristService;
            _context = applicationDbContext;
            _blobStorageService = blobStorageService;
        }

        [HttpPost("translate-audio/{placeName}/{touristName}")]
        public async Task<IActionResult> TranslateAudio(string placeName, string touristName)
        {
            try
            {
                string touristID = _touristService.GetUserIdByUsername(touristName);
                if (touristID == null)
                {
                    return NotFound($"Tourist with name {touristName} doesn't exist");
                }

                string targetLanguage = _context.Tourist
                                                .Where(t => t.Id == touristID)
                                                .FirstOrDefault()?.Language;

                var place = await _context.Places.Include(p => p.PlaceMedias)
                                                 .FirstOrDefaultAsync(p => p.PlaceName == placeName);
                if (place == null)
                {
                    return NotFound($"Place with name {placeName} not found");
                }

                var originalAudio = place.PlaceMedias.FirstOrDefault(m => m.MediaType == "audio");
                if (originalAudio == null)
                {
                    return NotFound($"Original audio for place {placeName} not found");
                }

                var audioBlobUrl = _blobStorageService.GetBlobUrlmedia(originalAudio.MediaContent);
                var localAudioFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", originalAudio.MediaContent);

                // Ensure the directory exists
                var directory = Path.GetDirectoryName(localAudioFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Download the audio file from Blob Storage
                await _blobStorageService.DownloadBlobAsync(originalAudio.MediaContent, localAudioFilePath);

                // Transcribe the audio file
                string transcriptionResult = await _audioTranscriptionService.TranscribeAudioAsync(localAudioFilePath);
                if (transcriptionResult.StartsWith("Error"))
                {
                    _logger.LogError($"Error during transcription: {transcriptionResult}");
                    return StatusCode(500, transcriptionResult);
                }

                string transcribedText = ExtractRawText(transcriptionResult);

                string audioPath;
                if (targetLanguage == "en")
                {
                    audioPath = localAudioFilePath;
                }
                else
                {
                    // Translate the transcribed text
                    string translationResult = await _translationService.TranslateTextAsync(transcribedText, targetLanguage);
                    if (translationResult == null)
                    {
                        _logger.LogError($"Error during translation: {transcriptionResult}");
                        return StatusCode(500, "Translation error.");
                    }

                    string translatedText = ExtractTranslatedText(translationResult);

                    // Convert the translated text to speech
                    audioPath = await _textToSpeechService.SynthesizeSpeechAsync(translatedText, targetLanguage);
                }

                var translatedAudioPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", audioPath);
                if (System.IO.File.Exists(translatedAudioPath))
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    var fullUrl = $"{baseUrl}/{audioPath.Replace("\\", "/")}";

                    // Delete the original file and its directory after the translated audio is processed
                    if (System.IO.File.Exists(localAudioFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(localAudioFilePath);
                            _logger.LogInformation($"Deleted original file: {localAudioFilePath}");

                            // Check for additional files (e.g., WAV files) in the directory and delete them
                            var directoryInfo = new DirectoryInfo(directory);
                            foreach (var file in directoryInfo.GetFiles())
                            {
                                if (file.Extension.ToLower() == ".wav")
                                {
                                    file.Delete();
                                    _logger.LogInformation($"Deleted additional file: {file.FullName}");
                                }
                            }

                            // Delete the folder if it is empty
                            if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
                            {
                                Directory.Delete(directory, true);
                                _logger.LogInformation($"Deleted empty folder: {directory}");

                                // Optionally, delete parent directories if they are empty
                                var parentDirectory = Directory.GetParent(directory);
                                while (parentDirectory != null && !Directory.EnumerateFileSystemEntries(parentDirectory.FullName).Any())
                                {
                                    Directory.Delete(parentDirectory.FullName, true);
                                    _logger.LogInformation($"Deleted empty folder: {parentDirectory.FullName}");
                                    parentDirectory = parentDirectory.Parent;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error deleting file or folder: {localAudioFilePath}");
                            // Optionally handle the error (e.g., log it, return a warning, etc.)
                        }
                    }

                    return Ok(new { path = fullUrl });
                }
                else
                {
                    _logger.LogError($"File not found: {translatedAudioPath}");
                    return NotFound();
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
