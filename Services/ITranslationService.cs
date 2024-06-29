namespace Guide_Me.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateTextAsync(string text, string targetLanguage);
        Task<string> TranslatedataAsync(string text, string targetLanguage);
        string TranslateTextResultASync(string text, string targetLanguage);
       
    }
}
