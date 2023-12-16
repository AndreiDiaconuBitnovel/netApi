namespace WebApplication2.Models.TranslateWatson
{
    public class TranslateWatsonRes
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? InputText { get; set; }
        public string? TranslatedText { get; set; }

        public TranslateWatsonRes(TranslateWatsonReq req, string TranslatedText)
        {
            From = req.From;
            To = req.To;
            InputText = req.InputText;
            this.TranslatedText = TranslatedText;
        }
    }
}
