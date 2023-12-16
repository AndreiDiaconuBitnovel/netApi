using WebApplication2.Models.TranslateWatson;

namespace WebApplication2.Services.TranslateWatson
{
    public interface ITranslateWatsonService
    {
        Task<TranslateWatsonRes> GetTranslation(TranslateWatsonReq req);
        Task<bool> Validate(TranslateWatsonReq req);
        Task<bool> PairsExist(TranslateWatsonReq req);
        List<TranslateWatsonRes> LoadUserResponses(string userId);
    }
}
