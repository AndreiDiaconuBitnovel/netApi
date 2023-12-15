using WebApplication2.DataDB;

namespace WebApplication2.Services.TranslateLanguageFolder
{
    public interface ITranslateLanguageService
    {
        Task<List<TranslateLanguage>> GetAllDataFromTranslateLanguageTable();
    }
}
