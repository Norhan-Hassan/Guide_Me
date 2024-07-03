using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Lame;

namespace Guide_Me.Services
{
    public class TextToSpeechService : ITextToSpeechService
    {
        private readonly string _subscriptionKey;
        private readonly string _region;

        public TextToSpeechService(IConfiguration configuration)
        {
            _subscriptionKey = configuration["AzureSpeech:SubscriptionKey"];
            _region = configuration["AzureSpeech:Region"];
            Console.WriteLine($"SubscriptionKey: {_subscriptionKey}, Region: {_region}");
        }

        public async Task<string> SynthesizeSpeechAsync(string text, string language)
        {
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audios");
            Directory.CreateDirectory(wwwrootPath);

            var wavFileName = GenerateFileName(text, ".wav");
            var wavFilePath = Path.Combine(wwwrootPath, wavFileName);

            Console.WriteLine($"WAV File Path: {wavFilePath}");

            var voiceName = GetVoiceName(language); // Get the appropriate voice name for the language

            var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            config.SpeechSynthesisVoiceName = voiceName;

            var audioConfig = AudioConfig.FromWavFileOutput(wavFilePath);
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);

            var result = await synthesizer.SpeakTextAsync(text);
            OutputSpeechSynthesisResult(result, text); // Log the result

            // Ensure proper disposal
            audioConfig.Dispose();
            synthesizer.Dispose();

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                var mp3FilePath = await ConvertWavToMp3Async(wavFilePath);

                // Delete the temporary WAV file
                if (File.Exists(wavFilePath))
                {
                    File.Delete(wavFilePath);
                    Console.WriteLine($"WAV File Deleted: {wavFilePath}");
                }

                return Path.Combine("audios", Path.GetFileName(mp3FilePath)); // Return the relative path to use in responses or storage
            }
            else
            {
                throw new Exception($"Speech synthesis failed: {result.Reason}");
            }
        }

        private string GenerateFileName(string text, string extension)
        {
            var sanitizedText = string.Join("_", text.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            if (sanitizedText.Length > 20)
            {
                sanitizedText = sanitizedText.Substring(0, 20);
            }
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{timestamp}{extension}";
        }

        private string GetVoiceName(string language)
        {
            // Define mappings of language codes to Azure Speech supported voice names
            var voiceMappings = new Dictionary<string, string>
                {
                    { "en", "en-US-AriaNeural" },      // English (United States)
                    { "ar", "ar-EG-SalmaNeural" },  // Arabic 
                    { "fr", "fr-FR-DeniseNeural" },    // French (France)
                    { "it", "it-IT-ElsaNeural" },        // Italian (Italy)
                    { "es", "es-ES-ElviraNeural" },    // Spanish (Spain)
                    { "de" , "de-DE-KatjaNeural"}, //
                    { "ja" , "ja-JP-NanamiNeural" } ,// japan
                    { "ru","ru-RU-SvetlanaNeural" },//Russia
                    { "zh","zh-CN-guangxi-YunqiNeural" }, //Chinese
                    // Add more mappings as needed for other languages supported by Azure Speech
                };

            // Default voice name if no specific mapping is found
            var defaultVoice = "en-US-AriaNeural"; // Replace with a suitable default voice

            // Try to find a matching voice name for the specified language
            if (voiceMappings.TryGetValue(language.ToLower(), out var voiceName))
            {
                return voiceName;
            }

            // Return default voice if no matching voice name is found
            return defaultVoice;
        }

        private void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task<string> ConvertWavToMp3Async(string wavFilePath)
        {
            try
            {
                var mp3FilePath = Path.ChangeExtension(wavFilePath, ".mp3");

                using (var reader = new WaveFileReader(wavFilePath))
                {
                    using (var writer = new LameMP3FileWriter(mp3FilePath, reader.WaveFormat, LAMEPreset.STANDARD))
                    {
                        await reader.CopyToAsync(writer);
                    }
                }

                // Optionally, delete the WAV file after successful conversion
                File.Delete(wavFilePath);

                return mp3FilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting WAV to MP3: {ex.Message}");
                throw; // Rethrow the exception to propagate it upwards
            }
        }
    }
}