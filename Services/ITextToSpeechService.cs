namespace Guide_Me.Services
{
    public interface ITextToSpeechService
    {
        Task<string> SynthesizeSpeechAsync(string text, string language);
    }
}
