﻿using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Guide_Me.Models;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace Guide_Me.Services
{
    public class AudioTranscriptionService : IAudioTranscriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureSpeechSettings _azureSpeechSettings;
        private readonly ILogger<AudioTranscriptionService> _logger;
        private readonly IPlaceService _placeService;
        private readonly IBlobStorageService _blobStorageService;

        public AudioTranscriptionService(
            ApplicationDbContext context,
            IOptions<AzureSpeechSettings> azureSpeechSettings,
            ILogger<AudioTranscriptionService> logger,
            IPlaceService placeService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _azureSpeechSettings = azureSpeechSettings.Value;
            _logger = logger;
            _placeService = placeService;
            _blobStorageService = blobStorageService;
        }

        public async Task<string> TranscribeAudioAsync(string audioFilePath)
        {
            if (!File.Exists(audioFilePath))
            {
                _logger.LogError($"File not found: {audioFilePath}");
                return $"Error: File not found: {audioFilePath}";
            }

            string wavFilePath = audioFilePath;

            // Convert MP3 to WAV if needed
            if (Path.GetExtension(audioFilePath).ToLower() == ".mp3")
            {
                wavFilePath = Path.ChangeExtension(audioFilePath, ".wav");
                ConvertMp3ToWav(audioFilePath, wavFilePath);
            }

            // Check if the file is a valid WAV file
            if (!IsValidWavFile(wavFilePath))
            {
                _logger.LogError($"Invalid WAV file: {wavFilePath}");
                return $"Error: Invalid WAV file: {wavFilePath}";
            }

            try
            {
                var config = SpeechConfig.FromSubscription(_azureSpeechSettings.SubscriptionKey, _azureSpeechSettings.Region);
                config.EndpointId = _azureSpeechSettings.Endpoint;

                using var audioConfig = AudioConfig.FromWavFileInput(wavFilePath);
                using var recognizer = new SpeechRecognizer(config, audioConfig);

                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    return result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    return "No speech could be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    _logger.LogError($"CANCELED: Reason={cancellation.Reason}, ErrorDetails={cancellation.ErrorDetails}");
                    return $"CANCELED: Reason={cancellation.Reason}, ErrorDetails={cancellation.ErrorDetails}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during transcription: {ex.Message}");
                return $"Error: {ex.Message}";
            }

            return string.Empty;
        }

        private bool IsValidWavFile(string filePath)
        {
            // Implement basic WAV file validation
            try
            {
                using var reader = new BinaryReader(File.OpenRead(filePath));
                var riff = reader.ReadInt32();
                var chunkSize = reader.ReadInt32();
                var wave = reader.ReadInt32();

                return riff == 0x46464952 && wave == 0x45564157; // "RIFF" and "WAVE" in ASCII
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating WAV file: {ex.Message}");
                return false;
            }
        }

        private void ConvertMp3ToWav(string mp3File, string wavFile)
        {
            try
            {
                using var reader = new Mp3FileReader(mp3File);
                using var writer = new WaveFileWriter(wavFile, reader.WaveFormat);
                reader.CopyTo(writer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error converting MP3 to WAV: {ex.Message}");
            }
        }

        public async Task<string> TranscribeSingleAudioFileAsync(string placeName)
        {
            var placeID = _placeService.GetPlaceIdByPlaceName(placeName);
            if (placeID == 0)
            {
                return $"Place {placeName} not found.";
            }
            var audioFile = await _context.placeMedias
                       .Where(m => m.PlaceId == placeID && m.MediaType == "audio").FirstOrDefaultAsync();

            if (audioFile == null)
            {
                return $"Audio file of place {placeName} not found.";
            }

            string localFilePath = Path.Combine(Path.GetTempPath(), audioFile.MediaContent);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(localFilePath));

            // Download the file from blob storage to a temporary location
            await _blobStorageService.DownloadBlobAsync(audioFile.MediaContent, localFilePath);

            // Transcribe the downloaded file
            string result = await TranscribeAudioAsync(localFilePath);

            // Optionally, delete the local file after transcription
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
            }

            if (!string.IsNullOrEmpty(result))
            {
                // Save the transcribed text as an audio file locally
                string transcribedAudioFilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(audioFile.MediaContent)}-transcribed.wav");
                SaveTranscriptionAsAudio(result, transcribedAudioFilePath);

                // Upload the transcribed audio file to blob storage
                using (var stream = new FileStream(transcribedAudioFilePath, FileMode.Open, FileAccess.Read))
                {
                    string blobUrl = await _blobStorageService.UploadBlobAsync("firstcontainer", Path.GetFileName(transcribedAudioFilePath), stream, "audio/mp3");
                    Console.WriteLine($"Transcribed audio file uploaded to: {blobUrl}");

                    // Optionally, delete the local transcribed audio file after uploading
                    if (File.Exists(transcribedAudioFilePath))
                    {
                        File.Delete(transcribedAudioFilePath);
                    }

                    return blobUrl;
                }
            }

            return result;
        }

        private void SaveTranscriptionAsAudio(string transcription, string filePath)
        {
            var config = SpeechConfig.FromSubscription(_azureSpeechSettings.SubscriptionKey, _azureSpeechSettings.Region);
            var synthesizer = new SpeechSynthesizer(config);

            using var stream = AudioConfig.FromWavFileOutput(filePath);
            synthesizer.SpeakTextAsync(transcription).Wait();
        }
    }
}
