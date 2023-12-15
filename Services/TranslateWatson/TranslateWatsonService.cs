using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using WebApplication2.DataDB;

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
            string translation = await translate(req);
            return new TranslateWatsonRes(req, translation);
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
            for(int i=0; i<res.Count; i++)
            {
                string code = res[i].Code;
                if(req.From == code) {
                    found1 = true;
                } else if(req.To == code) {
                    found2 = true;
                }
            }
            return found1 && found2;
        }
    }
}
