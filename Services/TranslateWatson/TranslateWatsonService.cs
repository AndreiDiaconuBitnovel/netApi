namespace WebApplication2.Services.TranslateWatson
{
    public class TranslateWatsonService : ITranslateWatsonService
    {
        public async Task<TranslateWatsonRes> GetTranslation(TranslateWatsonReq req)
        {
            String translation = "PLACEHOLDER TRANSLATION";
            // TODO: Call IBM Watson translation
            return new TranslateWatsonRes(req, translation);
        }
    }
}
