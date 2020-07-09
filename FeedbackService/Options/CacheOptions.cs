namespace FeedbackService.Options
{
    public class CacheOptions
    {
        public string Configuration { get; set; }
        public string ApplicationAlias { get; set; }
        public Expiry[] Expiry { get; set; }
    }
}
