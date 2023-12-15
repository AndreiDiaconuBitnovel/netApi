namespace WebApplication2.Services.TranslateWatson
{
    public class TranslateWatsonRes
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? InputText { get; set; }
        public string? TranslatedText { get; set; }

        public TranslateWatsonRes(TranslateWatsonReq req, String TranslatedText) {
            this.From = req.From;
            this.To = req.To;
            this.InputText = req.InputText;
            this.TranslatedText = TranslatedText;
        }
    }
}
