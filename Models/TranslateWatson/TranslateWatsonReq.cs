namespace WebApplication2.Models.TranslateWatson
{
    public class TranslateWatsonReq
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? InputText { get; set; }

        public string getLanguageModule()
        {
            return From + "-" + To;
        }

        public TranslateWatsonReq CopyRequest()
        {
            TranslateWatsonReq req1 = new TranslateWatsonReq();
            req1.From = From;
            req1.To = To;
            req1.InputText = InputText;
            return req1;
        }
    }
}
