namespace Guide_Me.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateTextAsync(string text, string targetLanguage);
    }
}
