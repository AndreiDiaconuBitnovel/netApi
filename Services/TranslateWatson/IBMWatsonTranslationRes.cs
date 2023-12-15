namespace WebApplication2.Services.TranslateWatson
{
    public class IBMWatsonTranslationRes
    {
        public int? word_count { get; set; }
        public int? character_count { get; set; }
        public List<IBMWatsonTranslationResInner> translations { get; set; }
    }

    public class IBMWatsonTranslationResInner
    {
        public string? translation { get; set; }
    }
}
