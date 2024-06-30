namespace Guide_Me.Services
{
    public interface IAudioTranscriptionService
    {
        Task<string> TranscribeAudioAsync(string audioFilePath);
        Task<string> TranscribeSingleAudioFileAsync(string placeName, HttpContext httpContext);
    }
}
