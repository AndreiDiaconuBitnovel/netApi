namespace WebApplication2.Services.TranslateWatson
{
    public interface ITranslateWatsonService
    {
        Task<TranslateWatsonRes> GetTranslation(TranslateWatsonReq req);
    }
}
