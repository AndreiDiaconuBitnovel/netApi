using System.Net;
using System.Text.Json;

namespace WebApplication2.Services.TranslateWatson
{
    public class TranslateWatsonService : ITranslateWatsonService
    {
        private String ibmWatsonInstanceUrl;
        private String ibmWatsonApiKey;
        private String ibmWatsonUser;
        public TranslateWatsonService(IConfiguration config) {
            ibmWatsonInstanceUrl = config.GetSection("IBMWatsonUrl").Value;
            ibmWatsonApiKey = config.GetSection("IBMWatsonApiKey").Value;
            ibmWatsonUser = config.GetSection("IBMWatsonUser").Value;
        }
        public async Task<TranslateWatsonRes> GetTranslation(TranslateWatsonReq req)
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
            return new TranslateWatsonRes(req, translation);
        }
    }
}
