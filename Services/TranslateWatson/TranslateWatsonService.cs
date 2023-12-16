using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using WebApplication2.DataDB;
using WebApplication2.Models.TranslateWatson;

namespace WebApplication2.Services.TranslateWatson
{
    public class TranslateWatsonService : ITranslateWatsonService
    {
        private readonly TeamDropDatabaseContext _testContext;

        private String ibmWatsonInstanceUrl;
        private String ibmWatsonApiKey;
        private String ibmWatsonUser;
        public TranslateWatsonService(IConfiguration config, TeamDropDatabaseContext testContext) {
            _testContext = testContext;
            ibmWatsonInstanceUrl = config.GetSection("IBMWatsonUrl").Value;
            ibmWatsonApiKey = config.GetSection("IBMWatsonApiKey").Value;
            ibmWatsonUser = config.GetSection("IBMWatsonUser").Value;
        }
        public async Task<TranslateWatsonRes> GetTranslation(TranslateWatsonReq req)
        {
            // Find user:
            bool userFound = _testContext.Users
                .Where(u => u.IdUser.ToString().ToUpper() == req.UserId.ToUpper())
                .Count() > 0;
            if (!userFound)
            {
                var errUserNotFound = new HttpRequestException(
                        "User not found.",
                        null,
                        System.Net.HttpStatusCode.NotFound
                    );
                throw errUserNotFound;
            }

            string en = "en";
            bool pairsExist = await PairsExist(req);
            var err = new HttpRequestException(
                    "Translation between these two languages is unavailable.",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            if (pairsExist)
            {
                // Direct translation available:
                string translation = await translate(req);
                TranslateWatsonRes translateWatsonRes = new TranslateWatsonRes(req, translation);
                SaveResponse(translateWatsonRes, req.UserId);
                return translateWatsonRes;
            } else {
                if (req.From == en || req.To == en)
                {
                    throw err;
                } else {
                    // Intermediare translation:
                    TranslateWatsonReq req1 = req.CopyRequest();
                    req1.To = en;
                    bool ok1 = await PairsExist(req1);
                    if (!ok1)
                    {
                        throw err;
                    }
                    string translation1 = await translate(req1);
                    // Final translation:
                    TranslateWatsonReq req2 = req.CopyRequest();
                    req2.From = en;
                    req2.InputText = translation1;
                    bool ok2 = await PairsExist(req2);
                    if (!ok2)
                    {
                        throw err;
                    }
                    string translation2 = await translate(req2);
                    TranslateWatsonRes translateWatsonRes = new TranslateWatsonRes(req, translation2);
                    SaveResponse(translateWatsonRes, req.UserId);
                    return translateWatsonRes;
                }
            }
        }

        private async Task<string> translate(TranslateWatsonReq req)
        {
            string translation;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ibmWatsonInstanceUrl + "/v3/translate?version=2018-05-01");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Credentials = new NetworkCredential(ibmWatsonUser, ibmWatsonApiKey);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"text\":[\"" + req.InputText + "\"],\"model_id\":\"" + req.getLanguageModule() + "\"}";
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string streamRes = streamReader.ReadToEnd();
                IBMWatsonTranslationRes result = JsonSerializer.Deserialize<IBMWatsonTranslationRes>(streamRes);
                translation = result.translations[0].translation;
            }
            return translation;
        }

        public async Task<bool> PairsExist(TranslateWatsonReq req)
        {
            List<TranslateLanguagePair> res = await _testContext.TranslateLanguagePairs.ToListAsync();
            foreach (TranslateLanguagePair pair in res) {
                if(req.From == pair.Source && req.To == pair.Target)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Validate(TranslateWatsonReq req)
        {
            // Don't try to translate into same language:
            if(req.From == req.To)
            {
                return false;
            }
            // Check that codes exist in DB:
            List<TranslateLanguage> res = await _testContext.TranslateLanguages.ToListAsync();
            bool found1 = false;
            bool found2 = false;
            foreach (TranslateLanguage language in res)
            {
                string code = language.Code;
                if(req.From == code) {
                    found1 = true;
                } else if(req.To == code) {
                    found2 = true;
                }
            }
            return found1 && found2;
        }
        
        private async void SaveResponse(TranslateWatsonRes res, string userId)
        {
            // TODO:
        }
    }
}
